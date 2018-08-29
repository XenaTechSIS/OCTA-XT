using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Security;
using FSP.Domain.Model;

namespace FSP.Common
{
    public class FSPMembershipProvider : MembershipProvider
    {
        private string _connectionString;
        private MachineKeySection _machineKey;
        private readonly int newPasswordLength = 8;

        #region properties

        public bool WriteExceptionsToEventLog { get; set; }

        private string _pApplicationName;
        private bool _pEnablePasswordReset;
        private bool _pEnablePasswordRetrieval;
        private bool _pRequiresQuestionAndAnswer;
        private bool _pRequiresUniqueEmail;
        private int _pMaxInvalidPasswordAttempts;
        private int _pPasswordAttemptWindow;
        private MembershipPasswordFormat _pPasswordFormat;

        public override string ApplicationName
        {
            get => _pApplicationName;
            set => _pApplicationName = value;
        }

        public override bool EnablePasswordReset => _pEnablePasswordReset;

        public override bool EnablePasswordRetrieval => _pEnablePasswordRetrieval;

        public override bool RequiresQuestionAndAnswer => _pRequiresQuestionAndAnswer;

        public override bool RequiresUniqueEmail => _pRequiresUniqueEmail;

        public override int MaxInvalidPasswordAttempts => _pMaxInvalidPasswordAttempts;

        public override int PasswordAttemptWindow => _pPasswordAttemptWindow;

        public override MembershipPasswordFormat PasswordFormat => _pPasswordFormat;

        private int _pMinRequiredNonAlphanumericCharacters;

        public override int MinRequiredNonAlphanumericCharacters => _pMinRequiredNonAlphanumericCharacters;

        private int _pMinRequiredPasswordLength;

        public override int MinRequiredPasswordLength => _pMinRequiredPasswordLength;

        private string _pPasswordStrengthRegularExpression;

        public override string PasswordStrengthRegularExpression => _pPasswordStrengthRegularExpression;

        #endregion

        #region methods

        /// <summary>
        ///     Initialize
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            _pApplicationName = GetConfigValue(config["applicationName"], HostingEnvironment.ApplicationVirtualPath);
            _pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            _pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            _pMinRequiredNonAlphanumericCharacters =
                Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"));
            _pMinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
            _pPasswordStrengthRegularExpression =
                Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
            _pEnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            _pEnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
            _pRequiresQuestionAndAnswer =
                Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
            _pRequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));
            WriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"));

            var tempFormat = config["passwordFormat"];
            if (tempFormat == null) tempFormat = "Hashed";

            switch (tempFormat)
            {
                case "Hashed":
                    _pPasswordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    _pPasswordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    _pPasswordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new ProviderException("Password format not supported.");
            }

            //
            // Initialize OdbcConnection.
            //

            var connectionStringSettings = ConfigurationManager.ConnectionStrings[config["connectionStringName"]];

            if (connectionStringSettings == null || connectionStringSettings.ConnectionString.Trim() == "")
                throw new ProviderException("Connection string cannot be blank.");

            _connectionString = connectionStringSettings.ConnectionString;


            // Get encryption and decryption key information from the configuration.
            var cfg = WebConfigurationManager.OpenWebConfiguration(HostingEnvironment.ApplicationVirtualPath);
            _machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");
            if (!_machineKey.ValidationKey.Contains("AutoGenerate")) return;
            if (PasswordFormat != MembershipPasswordFormat.Clear)
                throw new ProviderException("Hashed or Encrypted passwords " +
                                            "are not supported with auto-generated keys.");
        }

        private string GetConfigValue(string configValue, string defaultValue)
        {
            return string.IsNullOrEmpty(configValue) ? defaultValue : configValue;
        }


        public override MembershipUser CreateUser(string username,
            string password,
            string email,
            string passwordQuestion,
            string passwordAnswer,
            bool isApproved,
            object providerUserKey,
            out MembershipCreateStatus status)
        {
            var args =
                new ValidatePasswordEventArgs(email, password, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }


            if (RequiresUniqueEmail && GetUserNameByEmail(email) != "")
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            var u = GetUser(email, false);

            if (u == null)
            {
                if (providerUserKey == null)
                {
                    providerUserKey = Guid.NewGuid();
                }
                else
                {
                    if (!(providerUserKey is Guid))
                    {
                        status = MembershipCreateStatus.InvalidProviderUserKey;
                        return null;
                    }
                }

                try
                {
                    using (var db = new FSPDataContext())
                    {
                        var user = new User
                        {
                            Email = email,
                            Password = EncodePassword(password),
                            DateCreated = DateTime.Now,
                            IsApproved = isApproved
                        };
                        db.Users.InsertOnSubmit(user);
                        db.SubmitChanges();

                        status = MembershipCreateStatus.Success;
                    }
                }
                catch
                {
                    status = MembershipCreateStatus.ProviderError;
                }

                return GetUser(email, false);
            }

            status = MembershipCreateStatus.DuplicateUserName;


            return null;
        }


        public MembershipUser CreateFSPUser(Guid userId, string email, string password, Guid roleId, bool isApproved,
            out MembershipCreateStatus status,
            string firstName = "", string lastName = "", string address = "", string city = "", string state = "",
            string zip = "", string phoneNumber = "")
        {
            var args =
                new ValidatePasswordEventArgs(email, password, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }


            if (RequiresUniqueEmail && GetUserNameByEmail(email) != "")
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            var u = GetUser(email, false);

            if (u == null)
            {
                try
                {
                    using (var db = new FSPDataContext())
                    {
                        if (roleId == Guid.Empty)
                            roleId = db.Roles.FirstOrDefault(p => p.RoleName == "Viewer").RoleID;

                        var user = new User
                        {
                            RoleID = roleId,
                            UserID = userId,
                            Email = email,
                            FirstName = firstName,
                            LastName = lastName,
                            Password = EncodePassword(password),
                            DateCreated = DateTime.Now,
                            IsApproved = isApproved,
                            Address = address,
                            City = city,
                            State = state,
                            Zip = zip,
                            PhoneNumber = phoneNumber,
                            WantsNotification = true
                        };

                        db.Users.InsertOnSubmit(user);
                        db.SubmitChanges();


                        status = MembershipCreateStatus.Success;
                    }
                }
                catch
                {
                    status = MembershipCreateStatus.ProviderError;
                }

                return GetUser(email, false);
            }

            status = MembershipCreateStatus.DuplicateUserName;


            return null;
        }

        public override MembershipUser GetUser(string email, bool userIsOnline)
        {
            MembershipUser u = null;

            try
            {
                using (var db = new FSPDataContext())
                {
                    var users = from q in db.Users
                                where q.Email.Equals(email) && q.IsApproved == true
                                select q;


                    if (users.Any())
                    {
                        var user = users.FirstOrDefault();

                        u = GetUserFromReader(user);

                        if (userIsOnline)
                        {
                            user.LastActivityDate = DateTime.Today;
                            db.SubmitChanges();
                        }
                    }
                }
            }
            catch
            {
            }


            return u;
        }

        private MembershipUser GetUserFromReader(User user)
        {
            object userId = user.UserID;
            var email = user.Email;
            var firstName = user.FirstName;
            var lastName = user.LastName;

            var creationDate = Convert.ToDateTime(user.DateCreated);

            var lastLoginDate = new DateTime();
            if (user.LastLoginDate != null)
                lastLoginDate = Convert.ToDateTime(user.LastLoginDate);

            var lastActivityDate = new DateTime();
            if (user.LastActivityDate != null)
                lastActivityDate = Convert.ToDateTime(user.LastActivityDate);

            var u = new MembershipUser(Name,
                email,
                userId,
                email,
                "",
                "",
                true,
                false,
                creationDate,
                lastLoginDate,
                lastActivityDate,
                DateTime.MinValue,
                DateTime.MinValue);

            return u;
        }


        public override string GetUserNameByEmail(string email)
        {
            var userFullName = string.Empty;

            try
            {
                using (var db = new FSPDataContext())
                {
                    var users = from q in db.Users
                                where q.Email.Equals(email)
                                select q;

                    if (users.Any())
                    {
                        var user = users.FirstOrDefault();
                        userFullName = user.FirstName + " " + user.LastName;
                    }
                }
            }
            catch
            {
            }

            return userFullName;
        }

        public override bool ValidateUser(string email, string password)
        {

            try
            {
                using (var db = new FSPDataContext())
                {

                    var user = db.Users.FirstOrDefault(p => p.Email.Equals(email) && p.IsApproved == true);

                    if (user == null) return false;

                    if (!CheckPassword(password, user.Password)) return false;

                    user.LastLoginDate = DateTime.Now;
                    db.SubmitChanges();
                    return true;

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ValidateUser failed {ex.Message}");
                return false;
            }

        }


        #region password

        public override string ResetPassword(string username, string answer)
        {
            if (!EnablePasswordReset) throw new NotSupportedException("Password reset is not enabled.");

            if (answer == null && RequiresQuestionAndAnswer)
            {
                UpdateFailureCount(username, "passwordAnswer");

                throw new ProviderException("Password answer required for password reset.");
            }

            var newPassword =
                Membership.GeneratePassword(newPasswordLength, MinRequiredNonAlphanumericCharacters);


            var args =
                new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException(
                        "Reset password canceled due to password validation failure.");


            var conn = new OdbcConnection(_connectionString);
            var cmd = new OdbcCommand("SELECT PasswordAnswer, IsLockedOut FROM Users " +
                                      " WHERE Username = ? AND ApplicationName = ?", conn);

            cmd.Parameters.Add("@Username", OdbcType.VarChar, 255).Value = username;
            cmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = _pApplicationName;

            var rowsAffected = 0;
            var passwordAnswer = "";
            OdbcDataReader reader = null;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

                if (reader.HasRows)
                {
                    reader.Read();

                    if (reader.GetBoolean(1))
                        throw new MembershipPasswordException("The supplied user is locked out.");

                    passwordAnswer = reader.GetString(0);
                }
                else
                {
                    throw new MembershipPasswordException("The supplied user name is not found.");
                }

                if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
                {
                    UpdateFailureCount(username, "passwordAnswer");

                    throw new MembershipPasswordException("Incorrect password answer.");
                }

                var updateCmd = new OdbcCommand("UPDATE Users " +
                                                " SET Password = ?, LastPasswordChangedDate = ?" +
                                                " WHERE Username = ? AND ApplicationName = ? AND IsLockedOut = False",
                    conn);

                updateCmd.Parameters.Add("@Password", OdbcType.VarChar, 255).Value = EncodePassword(newPassword);
                updateCmd.Parameters.Add("@LastPasswordChangedDate", OdbcType.DateTime).Value = DateTime.Now;
                updateCmd.Parameters.Add("@Username", OdbcType.VarChar, 255).Value = username;
                updateCmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = _pApplicationName;

                rowsAffected = updateCmd.ExecuteNonQuery();
            }
            catch (OdbcException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    //WriteToEventLog(e, "ResetPassword");

                    //throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                conn.Close();
            }

            if (rowsAffected > 0)
                return newPassword;
            throw new MembershipPasswordException("User not found, or user is locked out. Password not Reset.");
        }

        public override string GetPassword(string username, string answer)
        {
            if (!EnablePasswordRetrieval) throw new ProviderException("Password Retrieval Not Enabled.");

            if (PasswordFormat == MembershipPasswordFormat.Hashed)
                throw new ProviderException("Cannot retrieve Hashed passwords.");

            var conn = new OdbcConnection(_connectionString);
            var cmd = new OdbcCommand("SELECT Password, PasswordAnswer, IsLockedOut FROM Users " +
                                      " WHERE Username = ? AND ApplicationName = ?", conn);

            cmd.Parameters.Add("@Username", OdbcType.VarChar, 255).Value = username;
            cmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = _pApplicationName;

            var password = "";
            var passwordAnswer = "";
            OdbcDataReader reader = null;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

                if (reader.HasRows)
                {
                    reader.Read();

                    if (reader.GetBoolean(2))
                        throw new MembershipPasswordException("The supplied user is locked out.");

                    password = reader.GetString(0);
                    passwordAnswer = reader.GetString(1);
                }
                else
                {
                    throw new MembershipPasswordException("The supplied user name is not found.");
                }
            }
            catch (OdbcException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    //WriteToEventLog(e, "GetPassword");

                    //throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                conn.Close();
            }


            if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
            {
                UpdateFailureCount(username, "passwordAnswer");

                throw new MembershipPasswordException("Incorrect password answer.");
            }


            if (PasswordFormat == MembershipPasswordFormat.Encrypted) password = UnEncodePassword(password);

            return password;
        }

        public override bool ChangePassword(string email, string oldPwd, string newPwd)
        {
            try
            {
                if (!ValidateUser(email, oldPwd))
                    return false;


                var args =
                    new ValidatePasswordEventArgs(email, newPwd, true);

                OnValidatingPassword(args);

                if (args.Cancel)
                    if (args.FailureInformation != null)
                        throw args.FailureInformation;
                    else
                        throw new MembershipPasswordException(
                            "Change password canceled due to new password validation failure.");

                using (var db = new FSPDataContext())
                {
                    var users = from q in db.Users
                                where q.Email.Equals(email) && q.Password.Equals(EncodePassword(oldPwd))
                                select q;

                    if (users.Count() > 0)
                    {
                        var user = users.FirstOrDefault();
                        user.Password = EncodePassword(newPwd);
                        db.SubmitChanges();

                        return true;
                    }

                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public override bool ChangePasswordQuestionAndAnswer(string username,
            string password,
            string newPwdQuestion,
            string newPwdAnswer)
        {
            if (!ValidateUser(username, password))
                return false;

            var conn = new OdbcConnection(_connectionString);
            var cmd = new OdbcCommand("UPDATE Users " +
                                      " SET PasswordQuestion = ?, PasswordAnswer = ?" +
                                      " WHERE Username = ? AND ApplicationName = ?", conn);

            cmd.Parameters.Add("@Question", OdbcType.VarChar, 255).Value = newPwdQuestion;
            cmd.Parameters.Add("@Answer", OdbcType.VarChar, 255).Value = EncodePassword(newPwdAnswer);
            cmd.Parameters.Add("@Username", OdbcType.VarChar, 255).Value = username;
            cmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = _pApplicationName;


            var rowsAffected = 0;

            try
            {
                conn.Open();

                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (OdbcException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    //WriteToEventLog(e, "ChangePasswordQuestionAndAnswer");
                    //throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }

            if (rowsAffected > 0) return true;

            return false;
        }

        private bool CheckPassword(string password, string dbpassword)
        {
            var pass1 = password;
            var pass2 = dbpassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    pass2 = UnEncodePassword(dbpassword);
                    break;
                case MembershipPasswordFormat.Hashed:
                    pass1 = EncodePassword(password);
                    break;
                default:
                    break;
            }

            if (pass1 == pass2) return true;

            return false;
        }

        public string EncodePassword(string password)
        {
            var encodedPassword = password;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPassword =
                        Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    var hash = new HMACSHA1();
                    hash.Key = HexToByte(_machineKey.ValidationKey);
                    encodedPassword =
                        Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
                    break;
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return encodedPassword;
        }

        public string UnEncodePassword(string encodedPassword)
        {
            var password = encodedPassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    password =
                        Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Cannot unencode a hashed password.");
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return password;
        }

        private byte[] HexToByte(string hexString)
        {
            var returnBytes = new byte[hexString.Length / 2];
            for (var i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        #endregion

        #region un-used

        public override MembershipUser GetUser(object userId, bool userIsOnline)
        {
            //SqlConnection conn = new SqlConnection(connectionString);
            //SqlCommand cmd = new SqlCommand("SELECT ApplicationUserId, Email, FirstName," +
            //      " LastName, PhoneNumber, Address, City, State," +
            //      " Zip, Country, WantsNotification" +
            //      " FROM ApplicationUsers WHERE ApplicationUserId = " + userId, conn);

            MembershipUser u = null;
            //SqlDataReader reader = null;

            //try
            //{
            //    conn.Open();

            //    reader = cmd.ExecuteReader();

            //    if (reader.HasRows)
            //    {
            //        reader.Read();
            //        u = GetUserFromReader(reader);

            //        if (userIsOnline)
            //        {
            //            SqlCommand updateCmd = new SqlCommand("UPDATE ApplicationUsers " +
            //                      "SET LastActivityDate = " + DateTime.Now + " WHERE ApplicationUserId = " + userId, conn);

            //            updateCmd.ExecuteNonQuery();
            //        }
            //    }

            //}
            //catch (OdbcException e)
            //{
            //    if (WriteExceptionsToEventLog)
            //    {
            //        //WriteToEventLog(e, "GetUser(Object, Boolean)");

            //        throw new ProviderException(exceptionMessage);
            //    }
            //    else
            //    {
            //        throw e;
            //    }
            //}
            //finally
            //{
            //    if (reader != null) { reader.Close(); }

            //    conn.Close();
            //}

            return u;
        }

        public override bool UnlockUser(string username)
        {
            var conn = new OdbcConnection(_connectionString);
            var cmd = new OdbcCommand("UPDATE Users " +
                                      " SET IsLockedOut = False, LastLockedOutDate = ? " +
                                      " WHERE Username = ? AND ApplicationName = ?", conn);

            cmd.Parameters.Add("@LastLockedOutDate", OdbcType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@Username", OdbcType.VarChar, 255).Value = username;
            cmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = _pApplicationName;

            var rowsAffected = 0;

            try
            {
                conn.Open();

                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (OdbcException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    //WriteToEventLog(e, "UnlockUser");

                    //throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }

            if (rowsAffected > 0)
                return true;

            return false;
        }

        public override void UpdateUser(MembershipUser user)
        {
            var conn = new OdbcConnection(_connectionString);
            var cmd = new OdbcCommand("UPDATE Users " +
                                      " SET Email = ?, Comment = ?," +
                                      " IsApproved = ?" +
                                      " WHERE Username = ? AND ApplicationName = ?", conn);

            cmd.Parameters.Add("@Email", OdbcType.VarChar, 128).Value = user.Email;
            cmd.Parameters.Add("@Comment", OdbcType.VarChar, 255).Value = user.Comment;
            cmd.Parameters.Add("@IsApproved", OdbcType.Bit).Value = user.IsApproved;
            cmd.Parameters.Add("@Username", OdbcType.VarChar, 255).Value = user.UserName;
            cmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = _pApplicationName;


            try
            {
                conn.Open();

                cmd.ExecuteNonQuery();
            }
            catch (OdbcException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    //WriteToEventLog(e, "UpdateUser");

                    //throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }
        }

        private void UpdateFailureCount(string username, string failureType)
        {
            var conn = new OdbcConnection(_connectionString);
            var cmd = new OdbcCommand("SELECT FailedPasswordAttemptCount, " +
                                      "  FailedPasswordAttemptWindowStart, " +
                                      "  FailedPasswordAnswerAttemptCount, " +
                                      "  FailedPasswordAnswerAttemptWindowStart " +
                                      "  FROM Users " +
                                      "  WHERE Username = ? AND ApplicationName = ?", conn);

            cmd.Parameters.Add("@Username", OdbcType.VarChar, 255).Value = username;
            cmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = _pApplicationName;

            OdbcDataReader reader = null;
            var windowStart = new DateTime();
            var failureCount = 0;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

                if (reader.HasRows)
                {
                    reader.Read();

                    if (failureType == "password")
                    {
                        failureCount = reader.GetInt32(0);
                        windowStart = reader.GetDateTime(1);
                    }

                    if (failureType == "passwordAnswer")
                    {
                        failureCount = reader.GetInt32(2);
                        windowStart = reader.GetDateTime(3);
                    }
                }

                reader.Close();

                var windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);

                if (failureCount == 0 || DateTime.Now > windowEnd)
                {
                    // First password failure or outside of PasswordAttemptWindow. 
                    // Start a new password failure count from 1 and a new window starting now.

                    if (failureType == "password")
                        cmd.CommandText = "UPDATE Users " +
                                          "  SET FailedPasswordAttemptCount = ?, " +
                                          "      FailedPasswordAttemptWindowStart = ? " +
                                          "  WHERE Username = ? AND ApplicationName = ?";

                    if (failureType == "passwordAnswer")
                        cmd.CommandText = "UPDATE Users " +
                                          "  SET FailedPasswordAnswerAttemptCount = ?, " +
                                          "      FailedPasswordAnswerAttemptWindowStart = ? " +
                                          "  WHERE Username = ? AND ApplicationName = ?";

                    cmd.Parameters.Clear();

                    cmd.Parameters.Add("@Count", OdbcType.Int).Value = 1;
                    cmd.Parameters.Add("@WindowStart", OdbcType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@Username", OdbcType.VarChar, 255).Value = username;
                    cmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = _pApplicationName;

                    if (cmd.ExecuteNonQuery() < 0)
                        throw new ProviderException("Unable to update failure count and window start.");
                }
                else
                {
                    if (failureCount++ >= MaxInvalidPasswordAttempts)
                    {
                        // Password attempts have exceeded the failure threshold. Lock out
                        // the user.

                        cmd.CommandText = "UPDATE Users " +
                                          "  SET IsLockedOut = ?, LastLockedOutDate = ? " +
                                          "  WHERE Username = ? AND ApplicationName = ?";

                        cmd.Parameters.Clear();

                        cmd.Parameters.Add("@IsLockedOut", OdbcType.Bit).Value = true;
                        cmd.Parameters.Add("@LastLockedOutDate", OdbcType.DateTime).Value = DateTime.Now;
                        cmd.Parameters.Add("@Username", OdbcType.VarChar, 255).Value = username;
                        cmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = _pApplicationName;

                        if (cmd.ExecuteNonQuery() < 0)
                            throw new ProviderException("Unable to lock out user.");
                    }
                    else
                    {
                        // Password attempts have not exceeded the failure threshold. Update
                        // the failure counts. Leave the window the same.

                        if (failureType == "password")
                            cmd.CommandText = "UPDATE Users " +
                                              "  SET FailedPasswordAttemptCount = ?" +
                                              "  WHERE Username = ? AND ApplicationName = ?";

                        if (failureType == "passwordAnswer")
                            cmd.CommandText = "UPDATE Users " +
                                              "  SET FailedPasswordAnswerAttemptCount = ?" +
                                              "  WHERE Username = ? AND ApplicationName = ?";

                        cmd.Parameters.Clear();

                        cmd.Parameters.Add("@Count", OdbcType.Int).Value = failureCount;
                        cmd.Parameters.Add("@Username", OdbcType.VarChar, 255).Value = username;
                        cmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = _pApplicationName;

                        if (cmd.ExecuteNonQuery() < 0)
                            throw new ProviderException("Unable to update failure count.");
                    }
                }
            }
            catch (OdbcException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    //WriteToEventLog(e, "UpdateFailureCount");

                    //throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                conn.Close();
            }
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            var conn = new OdbcConnection(_connectionString);
            var cmd = new OdbcCommand("DELETE FROM Users " +
                                      " WHERE Username = ? AND Applicationname = ?", conn);

            cmd.Parameters.Add("@Username", OdbcType.VarChar, 255).Value = username;
            cmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = _pApplicationName;

            var rowsAffected = 0;

            try
            {
                conn.Open();

                rowsAffected = cmd.ExecuteNonQuery();

                if (deleteAllRelatedData)
                {
                    // Process commands to delete all data for the user in the database.
                }
            }
            catch (OdbcException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    //WriteToEventLog(e, "DeleteUser");

                    //throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }

            if (rowsAffected > 0)
                return true;

            return false;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            //SqlConnection conn = new SqlConnection(connectionString);
            //SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM ApplicationUsers", conn);
            var users = new MembershipUserCollection();

            //SqlDataReader reader = null;
            //totalRecords = 0;

            //try
            //{
            //    conn.Open();
            totalRecords = 5; // (int)cmd.ExecuteScalar();

            //    if (totalRecords <= 0) { return users; }

            //    cmd.CommandText = "SELECT ApplicationUserId, Email, Password," +
            //             " FirstName, LastName, PhoneNumber, Address, City," +
            //             " State, Zip, Country " +
            //             " FROM ApplicationUsers " +
            //             " ORDER BY Email Asc";

            //    reader = cmd.ExecuteReader();

            //    int counter = 0;
            //    int startIndex = pageSize * pageIndex;
            //    int endIndex = startIndex + pageSize - 1;

            //    while (reader.Read())
            //    {
            //        if (counter >= startIndex)
            //        {
            //            MembershipUser u = GetUserFromReader(reader);
            //            users.Add(u);
            //        }

            //        if (counter >= endIndex) { cmd.Cancel(); }

            //        counter++;
            //    }
            //}
            //catch (OdbcException e)
            //{
            //    if (WriteExceptionsToEventLog)
            //    {
            //        //WriteToEventLog(e, "GetAllUsers ");

            //        throw new ProviderException(exceptionMessage);
            //    }
            //    else
            //    {
            //        throw e;
            //    }
            //}
            //finally
            //{
            //    if (reader != null) { reader.Close(); }
            //    conn.Close();
            //}

            return users;
        }

        public override int GetNumberOfUsersOnline()
        {
            var onlineSpan = new TimeSpan(0, Membership.UserIsOnlineTimeWindow, 0);
            var compareTime = DateTime.Now.Subtract(onlineSpan);

            var conn = new OdbcConnection(_connectionString);
            var cmd = new OdbcCommand("SELECT Count(*) FROM Users " +
                                      " WHERE LastActivityDate > ? AND ApplicationName = ?", conn);

            cmd.Parameters.Add("@CompareDate", OdbcType.DateTime).Value = compareTime;
            cmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = _pApplicationName;

            var numOnline = 0;

            try
            {
                conn.Open();

                numOnline = (int)cmd.ExecuteScalar();
            }
            catch (OdbcException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    //WriteToEventLog(e, "GetNumberOfUsersOnline");

                    //throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }

            return numOnline;
        }

        public override MembershipUserCollection FindUsersByName(string emailToMatch, int pageIndex, int pageSize,
            out int totalRecords)
        {
            //SqlConnection conn = new SqlConnection(connectionString);
            //SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM ApplicationUsers WHERE Username LIKE '" + emailToMatch + "'", conn);
            var users = new MembershipUserCollection();

            //SqlDataReader reader = null;

            //try
            //{
            //    conn.Open();
            totalRecords = 5; // (int)cmd.ExecuteScalar();

            //    if (totalRecords <= 0) { return users; }

            //    cmd.CommandText = "SELECT ApplicationUserId, Email, Password," +
            //           " FirstName, LastName, PhoneNumber, Address, City," +
            //           " State, Zip, Country " +
            //           " FROM ApplicationUsers " +
            //            " WHERE Username LIKE '" + emailToMatch + "'" +
            //           " ORDER BY Email Asc";

            //    reader = cmd.ExecuteReader();

            //    int counter = 0;
            //    int startIndex = pageSize * pageIndex;
            //    int endIndex = startIndex + pageSize - 1;

            //    while (reader.Read())
            //    {
            //        if (counter >= startIndex)
            //        {
            //            MembershipUser u = GetUserFromReader(reader);
            //            users.Add(u);
            //        }

            //        if (counter >= endIndex) { cmd.Cancel(); }

            //        counter++;
            //    }
            //}
            //catch (OdbcException e)
            //{
            //    if (WriteExceptionsToEventLog)
            //    {
            //        //WriteToEventLog(e, "FindUsersByName");

            //        throw new ProviderException(exceptionMessage);
            //    }
            //    else
            //    {
            //        throw e;
            //    }
            //}
            //finally
            //{
            //    if (reader != null) { reader.Close(); }

            //    conn.Close();
            //}

            return users;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize,
            out int totalRecords)
        {
            //SqlConnection conn = new SqlConnection(connectionString);
            //SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM ApplicationUsers " +
            //                                  "WHERE Email LIKE '" + emailToMatch + "'", conn);
            var users = new MembershipUserCollection();

            //SqlDataReader reader = null;
            //totalRecords = 0;

            //try
            //{
            //    conn.Open();
            totalRecords = 5; // (int)cmd.ExecuteScalar();

            //    if (totalRecords <= 0) { return users; }

            //    cmd.CommandText = "SELECT ApplicationUserId, Email, FirstName," +
            //             " LastName, PhoneNumber, Address, City, State," +
            //             " Zip, Country, WantsNotification " +
            //             " FROM ApplicationUsers " +
            //             " WHERE Email LIKE '" + emailToMatch + "'" +
            //             " ORDER BY Email Asc";

            //    reader = cmd.ExecuteReader();

            //    int counter = 0;
            //    int startIndex = pageSize * pageIndex;
            //    int endIndex = startIndex + pageSize - 1;

            //    while (reader.Read())
            //    {
            //        if (counter >= startIndex)
            //        {
            //            MembershipUser u = GetUserFromReader(reader);
            //            users.Add(u);
            //        }

            //        if (counter >= endIndex) { cmd.Cancel(); }

            //        counter++;
            //    }
            //}
            //catch (OdbcException e)
            //{
            //    if (WriteExceptionsToEventLog)
            //    {
            //        //WriteToEventLog(e, "FindUsersByEmail");

            //        throw new ProviderException(exceptionMessage);
            //    }
            //    else
            //    {
            //        throw e;
            //    }
            //}
            //finally
            //{
            //    if (reader != null) { reader.Close(); }

            //    conn.Close();
            //}

            return users;
        }

        #endregion

        #endregion
    }
}