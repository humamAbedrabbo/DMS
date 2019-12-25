// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function closeDialog(id_dialog) {
    $(id_dialog).modal('hide');
}

function showTab(id_tab) {
    $(id_tab).tab('show');
}

function onUploadFormReady() {
    document.getElementById('uploadFile').addEventListener('change', handleFileSelect, false);

    function handleFileSelect(evt) {
        var files = evt.target.files;
        var f = files[0];
        if (f) {
            if (f.type.search('image') >= 0) {
                var reader = new FileReader();

                reader.onload = (function (theFile) {

                    return function (e) {
                        // document.getElementById('imgContainer').innerHTML = ['<img id="img" src="', e.target.result, '" title="', theFile.name, '" width="150" />'].join('');
                        var img = new Image();

                        img.onload = function () {
                            var canvas = document.createElement("canvas");
                            canvas.id = "thumbnail";
                            new thumbnailer(canvas, img, 188, 3);

                            $("#imgContainer").empty();
                            $("#imgContainer").append(canvas);
                        };
                        img.src = e.target.result;
                    };
                })(f);

                reader.readAsDataURL(f);
            }
        }

    }
}

function btnUpload() {
    var $file = $("#uploadFile")[0].files[0];
    var $repositoryId = $("#repositoryId").val();
    var $title = $("#title").val();
    var $path = $("#path").val();
    var $userName = $("#userName").val();
    var $description = $("#description").val();
    var $parentId = $("#parentId").val();


    // let postUrl = postDocumentBaseUrl + $repositoryId + '/' + $parentId;
    let postUrl = "Archive/PostDocument";
    var FD = new FormData();
    FD.append('repositoryId', $repositoryId);
    FD.append('name', $file.name);
    FD.append('title', $title);
    FD.append('parentId', $parentId);
    FD.append('path', $path);
    FD.append('description', $description);
    FD.append('contentType', $file.type);
    FD.append('length', $file.size);
    FD.append('userName', $userName);

    $.ajax({
        type: "POST",
        url: postUrl,
        contentType: false,
        processData: false,
        dataType: 'json',
        data: FD,
        success: function (result) {
            console.log(result);
            
            $("#documentId").val(result.id);
            $("#checkInKey").val(result.checkInKey);
            $("#version").val(result.version);
            // UploadFile($('#uploadFile')[0].files);
        },
        failure: function (errmsg) {
            
            console.log(errmsg);
        }
    });
}