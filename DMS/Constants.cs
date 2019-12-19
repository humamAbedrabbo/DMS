using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS
{
    public static class Constants
    {
        public const string ROLE_ADMIN = "Admin";
        public const string ROLE_ARCHIVE = "Archive";
        public const string ROLE_PUBLIC = "Public";
        public const string USER_ADMIN = "admin";
        public const string USER_ADMIN_EMAIL = "admin@dms";
        public const string USER_DEFAULT_PWD = "123456";

        public const int USERNAME_MAX_LENGTH = 256;
        public const int REPO_NAME_MAX_LENGTH = 100;
        public const int REPO_DESC_MAX_LENGTH = 250;

        public const int FOLDER_NAME_MAX_LENGTH = 248;
        public const int FOLDER_DESC_MAX_LENGTH = 250;

        public const int METAFIELD_NAME_MAX_LENGTH = 50;
        public const int METAFIELD_TITLE_MAX_LENGTH = 100;
        public const int METAFIELD_DESC_MAX_LENGTH = 250;

        public const int DOCUMENT_NAME_MAX_LENGTH = 259;
        public const int DOCUMENT_TITLE_MAX_LENGTH = 300;
        public const int DOCUMENT_DESC_MAX_LENGTH = 500;
    }
}
