function UploadFile(TargetFile) {
    // create array to store the buffer chunks
    var FileChunk = [];
    // the file object itself that we will work with
    var file = TargetFile[0];

    // set up other initial vars
    var MaxFileSizeMB = 1;
    var BufferChunkSize = MaxFileSizeMB * (1024 * 1024);
    var ReadBuffer_Size = 1024;
    var FileStreamPos = 0;
    // set the initial chunk length
    var EndPos = BufferChunkSize;
    var Size = file.size;
    var SortId = 1;

    var documentId = $("#documentId").val();
    var repositoryId = $("#repositoryId").val();
    var userName = $("#userName").val();
    var checkInKey = $("#checkInKey").val();

    // add to the FileChunk array until we get to the end of the file
    while (FileStreamPos < Size) {
        // "slice" the file from the starting position/offset, to  the required length
        FileChunk.push(file.slice(FileStreamPos, EndPos));
        FileStreamPos = EndPos; // jump by the amount read
        EndPos = FileStreamPos + BufferChunkSize; // set next chunk length
    }
    // get total number of "files" we will be sending
    var TotalParts = FileChunk.length;
    var PartCount = 0;
    // loop through, pulling the first item from the array each time and sending it
    while (chunk = FileChunk.shift()) {
        PartCount++;
        // file name convention
        var FilePartName = file.name + ".part_" + PartCount + "." + TotalParts;
        // send the file
        UploadFileChunk(chunk, FilePartName, PartCount, file.name, repositoryId, documentId, userName, checkInKey);
    }
}

function UploadFileChunk(Chunk, FileName, PartCount, originalName, repositoryId, documentId, userName, checkInKey) {

    var FD = new FormData();
    FD.append('file', Chunk, FileName);
    FD.append('repositoryId', repositoryId);
    FD.append('documentId', documentId);
    FD.append('version', 0);
    FD.append('sortId', PartCount);
    FD.append('originalName', originalName);
    FD.append('userName', userName);
    FD.append('checkInKey', checkInKey);
    $.ajax({
        type: "POST",
        url: "Archive/UploadChunk",
        contentType: false,
        processData: false,
        data: FD,
        success: function (result) {
            
            console.log(result);
            if (result) {
                alert('done');
                //UploadThumbnail(documentId);
                //let $downloadBtn = $("#btnDownload");
                //let downloadUrl = downloadBaseUrl + documentId;
                //$downloadBtn.attr('href', downloadUrl);
                //$downloadBtn.removeAttr('disabled');
                //$downloadBtn.removeClass('disabled');
            }
        },
        failure: function (errmsg) {
           
            console.log(errmsg);
        }
    });
}

function UploadThumbnail(documentId) {

    var f = document.getElementById('uploadFile').files[0];
    if (f) {
        if (f.type.search('image') < 0) {
            return;
        }
    }
    var file = document.getElementById("thumbnail").toDataURL();

    var formdata = new FormData();
    formdata.append("documentId", documentId);
    formdata.append("base64image", file);

    $.ajax({
        url: "Archive/UploadThumbnail",
        type: "POST",
        data: formdata,
        processData: false,
        contentType: false,
        //success: function (result) {
        //    $.ajax({
        //        url: getDocumentBaseUrl + documentId,
        //        type: "GET",
        //        success: function (r) {
        //            debugger;
        //            document.getElementById("docThumbnail").src = r.thumbnail;
        //        }

        //    });
        //}
    });
}

// returns a function that calculates lanczos weight
function lanczosCreate(lobes) {
    return function (x) {
        if (x > lobes)
            return 0;
        x *= Math.PI;
        if (Math.abs(x) < 1e-16)
            return 1;
        var xx = x / lobes;
        return Math.sin(x) * Math.sin(xx) / x / xx;
    };
}

// elem: canvas element, img: image element, sx: scaled width, lobes: kernel radius
function thumbnailer(elem, img, sx, lobes) {
    this.canvas = elem;
    elem.width = img.width;
    elem.height = img.height;
    elem.style.display = "none";
    this.ctx = elem.getContext("2d");
    this.ctx.drawImage(img, 0, 0);
    this.img = img;
    this.src = this.ctx.getImageData(0, 0, img.width, img.height);
    this.dest = {
        width: sx,
        height: Math.round(img.height * sx / img.width),
    };
    this.dest.data = new Array(this.dest.width * this.dest.height * 3);
    this.lanczos = lanczosCreate(lobes);
    this.ratio = img.width / sx;
    this.rcp_ratio = 2 / this.ratio;
    this.range2 = Math.ceil(this.ratio * lobes / 2);
    this.cacheLanc = {};
    this.center = {};
    this.icenter = {};
    setTimeout(this.process1, 0, this, 0);
}

thumbnailer.prototype.process1 = function (self, u) {
    self.center.x = (u + 0.5) * self.ratio;
    self.icenter.x = Math.floor(self.center.x);
    for (var v = 0; v < self.dest.height; v++) {
        self.center.y = (v + 0.5) * self.ratio;
        self.icenter.y = Math.floor(self.center.y);
        var a, r, g, b;
        a = r = g = b = 0;
        for (var i = self.icenter.x - self.range2; i <= self.icenter.x + self.range2; i++) {
            if (i < 0 || i >= self.src.width)
                continue;
            var f_x = Math.floor(1000 * Math.abs(i - self.center.x));
            if (!self.cacheLanc[f_x])
                self.cacheLanc[f_x] = {};
            for (var j = self.icenter.y - self.range2; j <= self.icenter.y + self.range2; j++) {
                if (j < 0 || j >= self.src.height)
                    continue;
                var f_y = Math.floor(1000 * Math.abs(j - self.center.y));
                if (self.cacheLanc[f_x][f_y] == undefined)
                    self.cacheLanc[f_x][f_y] = self.lanczos(Math.sqrt(Math.pow(f_x * self.rcp_ratio, 2)
                        + Math.pow(f_y * self.rcp_ratio, 2)) / 1000);
                weight = self.cacheLanc[f_x][f_y];
                if (weight > 0) {
                    var idx = (j * self.src.width + i) * 4;
                    a += weight;
                    r += weight * self.src.data[idx];
                    g += weight * self.src.data[idx + 1];
                    b += weight * self.src.data[idx + 2];
                }
            }
        }
        var idx = (v * self.dest.width + u) * 3;
        self.dest.data[idx] = r / a;
        self.dest.data[idx + 1] = g / a;
        self.dest.data[idx + 2] = b / a;
    }

    if (++u < self.dest.width)
        setTimeout(self.process1, 0, self, u);
    else
        setTimeout(self.process2, 0, self);
};
thumbnailer.prototype.process2 = function (self) {
    self.canvas.width = self.dest.width;
    self.canvas.height = self.dest.height;
    self.ctx.drawImage(self.img, 0, 0, self.dest.width, self.dest.height);
    self.src = self.ctx.getImageData(0, 0, self.dest.width, self.dest.height);
    var idx, idx2;
    for (var i = 0; i < self.dest.width; i++) {
        for (var j = 0; j < self.dest.height; j++) {
            idx = (j * self.dest.width + i) * 3;
            idx2 = (j * self.dest.width + i) * 4;
            self.src.data[idx2] = self.dest.data[idx];
            self.src.data[idx2 + 1] = self.dest.data[idx + 1];
            self.src.data[idx2 + 2] = self.dest.data[idx + 2];
        }
    }
    self.ctx.putImageData(self.src, 0, 0);
    self.canvas.style.display = "block";
};