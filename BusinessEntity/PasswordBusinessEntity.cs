using System;


namespace BusinessEntity
{
    public class PasswordBusinessEntity
    {
        public string recovery_user_lgn { get; set; }
        public string recovery_user_email { get; set; }
        public string recovery_userExists { get; set; }
        public Guid recovery_guid { get; set; }
        public int recovery_diff { get; set; }

        public string recovery_new_pswd { get; set; }
        public string recovery_confirm_pswd { get; set; }
        public string recovery_token { get; set; }
    }
}
