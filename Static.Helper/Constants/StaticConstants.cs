using Static.Helper.Constants.EnumCollections;

namespace Static.Helper.Constants;

public static class StaticConstants
{
    public static string EnvironmentId()
    {
        if (string.IsNullOrEmpty(DeploymentEnv))
            return "1";
        else if (DeploymentEnv.ToLower().Contains("production"))
            return "0";
        else if (DeploymentEnv.ToLower().Contains("staging"))
            return "2";
        else
            return "1";
    }

    public enum Status
    {
        InActive = 0,
        Active = 1,
        Pending = 2,
    }

    public enum BookTypeAccessType
    {
        Teacher = 0,
        Student = 1,
        Teacher_Student = 2
    }

    public static string IvyWebhookSecret { get; set; }

    public static string TestpaperToken { get; set; }

    public const string EBookAsset = "EBook";

    public static readonly string WebSocketEncryptionKey =
        Environment.GetEnvironmentVariable("Web_Socket_Encription_Key") ?? string.Empty;

    public enum Localization
    {
        Nepali = 1,
        English = 2
    }

    public static class Platform
    {
        public const string StaticBackoffice = "StaticBackOffice";
        public const string StaticEndUser = "StaticEndUser";
        public const string PublisherAdmin = "BO-LITE";
        public const string StaticFrontEnd = "web-app";
    }

    public static class QueueType
    {
        public const string EmailQueue = "EMAIL";
        public const string TeachingResource = "TEACHINGRESOURCE";
        public const string UserNotification = "USERNOTIFICATION";
    }

    public static readonly string BizEnv = Environment.GetEnvironmentVariable("BIZ_ENV") ?? string.Empty;
    public static readonly string DeploymentEnv = Environment.GetEnvironmentVariable("DEPLOYMENT_ENV") ?? string.Empty;


    public static readonly bool IsNotProduction =
        new string[] { "development", "staging", "testing" }.Contains(DeploymentEnv);

    public abstract class UserInformation
    {
        public const int MaxUserNameOrEmailAddressLength = 120;
        public const int MinPhoneNumberLength = 7;
        public const int MaxPhoneNumberLength = 14;
    }

    public abstract class ErrorMessage
    {
        public const string InvalidPassword = "Your Email or Password is invalid!";
        public const string UserLocked = "User is locked out due to multiple failed attempts!";
        public const string AccessToken = "Invalid Access Token!";
        public const string InvalidEmail = "Invalid Email!";
        public const string SuspiciousUserException =
            "All videos have been locked due to a detected suspicious event. Please contact support at +977-9801010144 or +977-9801010155!";

        public const string PasswordNull = "Please setup your password.";
        public const string DeactivatedAccount = "Your account has been deactivated, please contact support.";
        public const string UnAuthorizedAccess = "UnAuthorized to perform this action!";
        public const string UnAuthorized = "UnAuthorized user!";
        public const string AlreadyLinked = "A User cannot link more than one external account for a provider!";
        public const string ExternalLinked = "External Account already linked to a User!";
        public const string InValidToken = "Invalid Token!";
        public const string InValidPhoneNumber = "Invalid Phone Number";
    }

    public static readonly string IsPublisher = Environment.GetEnvironmentVariable("ISPUBLISHER_ENV") ?? string.Empty;

    public abstract class StaticAsset
    {
        public const string EBookAsset = "EBook";
        public const string IVyAsset = "IVy";
    }


    public static partial class BookTypes
    {
        public const string TeacherEbook = "Teacher's Guide";
    }

    public enum ResourceAccessType
    {
        None = 0,
        Student = 1,
        Parent = 2,
        Teacher = 3,
        ParentStudent = 4,
        TeacherStudent = 5
    }

    public const int Complete = 100;
    public const string SetPasswordAgent = "Set-Password-User-Agent";

    public const string FileUploadFailedMessage = "Sorry, the file was not uploaded. Please try again.";
    public const string FileUploadSuccessMessage = "File uploaded successfully.";

    public static class GradeStatus
    {
        public const string Graded = "graded";
        public const string UnGraded = "unGraded";
    }
    
    public enum  AttachmentType
    {
        Link = 1,
        File = 2
    }
    
    public static class NotificationTypes
    {
        public const string Report = "REPORT";
        public const string PackageSubscription = "PACKAGESUBSCRIPTION";
        public const string Announcement = "ANNOUNCEMENT";
    }

    public static readonly string[] AllowedUploadFileType =
    [
        ".txt", ".doc", ".docx", ".pdf", ".xls", ".xlsx", ".jpg", ".png", ".jpeg", ".csv", ".ppt", ".pptx", ".zip"
    ];

    public static readonly string[] TeachingGuideAllowedUploadFileType = new[] { ".pdf" };
    public const long DefaultFileUploadSize = 26214400;

    public enum PackageStatus
    {
        Draft = 0,
        Published = 1,
        Inactive = 2,
        All = -1
    }

    public enum PackageFilter
    {
        NotPublished = 0,
        Published = 1
    }


    public enum TcOperation
    {
        Initial = 0,
        Addition = 1,
        Subtraction = 2
    }

    public static class CreditHistoryRemarks
    {
        public const string OpeningBalance = "Opening Balance.";
        public const string AddedByAdmin = "Credit added by admin : ";
        public const string DeductedByAdmin = "Credit deducted by admin : ";
        public const string AddedForPurchasingPackage = "Credit added by purchasing package :";
        public const string FromPaypal = "Added via paypal.";
        public const string FromStripe = "Added via stripe.";
        public const string UnlockAnswer = "Credit deducted for unlocking an answer.";
        public const string GetAcademicSupport = "Credit deducted for get academic support.";
        public const string GenerateSubjectiveTestPaper = "Credit deducted for generating subjective testpaper : ";
        public const string GenerateObjectiveTestPaper = "Credit deducted for generating objective testpaper : ";
        public const string SolveAcademicSupportQuery = "Credit Added for academic support query : ";
        public const string DeductedForGetOneAnswer = "Credit deducted for get one answer testpaper.";
        public const string RefundedForAcademicSupport = "Credit refunded for academic support.";
    }

    public static class TestPaper
    {
        public const string ClientEnv = "static";
        public const string ExpiryTime = "2h";
        public const string ReadyStatus = "READY";
        public const string DraftStatus = "DRAFT";
    }

    public enum TutorCreditSource
    {
        TestPaper = 1,
        SubjectPackage = 2,
        Paypal = 3,
        Stripe = 4,
        Admin = 5,
        AcademicSupport = 6,
        Self = 7
    }

    public enum PurchaseType
    {
        Package = 1,
        Custom = 2,
        TestPaper = 3
    }

    public enum PaymentType
    {
        Khalti = 1,
        Esewa = 2,
        Admin = 3,
        FonePay = 4,
        Paypal = 5,
        Stripe = 6
    }

    public const string Paypal = "Paypal";
    public const string Stripe = "Stripe";

    public enum PaymentStatus
    {
        PaymentInitiated = 1,
        Cancelled = 2,
        Processing = 3,
        Success = 4,
        Refunded = 5,
        Pending = 6,
        Expired = 7
    }

    public static class TransactionType
    {
        public const string Package = "Premium Package";
        public const string TutorCredit = "Tutor Credits";
        public const string Custom = "Custom Package";
    }

    public static string NotificationToken { get; set; }

    public const string TcPurchased = "TC Purchased";
    public const string CustomPackage = "Custom Package";
    public const string ContentExpertLeadEmail = "rachana.thapa@innovatetech.co";
    public const string ContentExpertLeadFullname = "Rachana Thapa";

    public const string StaticSupportEmail = "support@mysecondteacher.com";

    public enum ResourceType
    {
        MyResource = 1,
        GlobalResource = 2,
        SchoolResource = 3
    }

    public enum FileUploadType
    {
        Folder = 1,
        File = 2,
        Link = 3
    }

    public static class ResourceTypes
    {
        public const string MyResource = "MyResource";
        public const int ZeroVal = 0;
        public const string SchoolResource = "SchoolResource";
        public const int DefaultLevel = 1;
        public const int MaxDepth = 5;
        public const int Parent = 0;
        public const int Student = 1;
        public const string GlobalResource = "GlobalResource";
    }

    private static readonly string[] SizeSuffix = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

    public static string FileSizeWithSuffix(double bytes)
    {
        const int decimalPlace = 0;
        if (bytes < 0)
        {
            throw new ArgumentException("File size should be greater than zero");
        }

        var mag = (int)Math.Max(0, Math.Log(bytes, 1024));
        var adjustedSize = Math.Round(bytes / Math.Pow(1024, mag), decimalPlace);
        return $"{adjustedSize} {SizeSuffix[mag]}";
    }

    public static readonly List<string> BookTypesOrder = new List<string> { "textbook", "workbook", "teacher's guide" };

    public static class SubscriptionStatus
    {
        public const string Active = "Active";
        public const string Expired = "Expired";
        public const string ExpiresSoon = "Expires Soon";
    }

    public static class RabbitMq
    {
        public const string EmailQueue = "EMAILQUEUE";
        public const string NotificationQueue = "NOTIFICATIONQUEUE";
        public const string FcmQueue = "FCMQUEUE";
        public const string ContentExpertQueue = "CONTENTEXPERTQUEUE";
    }

    private static string TemplatePrefix()
    {
        var prefix =
            BizEnv is MstFork.Nepal or MstFork.Indonesia
                ? BizEnv.Substring(0, 1).ToUpper() + BizEnv.Substring(1).ToLower()
                : string.Empty;
        return prefix;
    }

    public static class EmailTemplate
    {
        public static readonly string StudentForgotPassword = TemplatePrefix() + "StaticStudentForgotPassword";
        public static readonly string ParentAndCeForgotPassword = TemplatePrefix() + "StaticParentAndCeForgotPassword";
        public static readonly string OtpDetail = TemplatePrefix() + "StaticOtpDetail";
        public static readonly string UserLockout = TemplatePrefix() + "StaticUserLockout";

        public static readonly string CompleteStudentRegistration =
            TemplatePrefix() + "StaticCompleteStudentRegistration";

        public static readonly string ParentAccountActivate = TemplatePrefix() + "StaticParentAccountActivate";
        public static readonly string ParentAccountDeactivate = TemplatePrefix() + "StaticParentAccountDeactivate";
        public static readonly string B2CStudentFirstRegistration = TemplatePrefix() + "B2CStudentFirstRegistration";

        public static readonly string B2CStudentCompleteRegistrationNew =
            TemplatePrefix() + "B2CStudentCompleteRegistrationNew";

        public static readonly string GeneralEmail = TemplatePrefix() + "General";
        public static readonly string NonStudentRegistration = TemplatePrefix() + "NonStudentRegistration";
        public static readonly string AlstonActivateAccount = TemplatePrefix() + "ActivateAccount";
        public static readonly string AlstonDeactivateAccount = TemplatePrefix() + "DeactivateAccount";
        public static readonly string ActivateAccount = TemplatePrefix() + "ActivateAccount";
        public static readonly string DeactivateAccount = TemplatePrefix() + "DeactivateAccount";

        public static readonly string AlstonParentRegistration = TemplatePrefix() + "ParentRegistration";
        public static readonly string AlstonParentRegistrationNew = TemplatePrefix() + "ParentRegistrationNew";
        public static readonly string ParentRegistration = TemplatePrefix() + "ParentRegistration";
        public static readonly string ParentRegistrationNew = TemplatePrefix() + "ParentRegistrationNew";

        public static readonly string B2CStudentWelcome = TemplatePrefix() + "B2CStudentWelcome";
        public static readonly string AlstonB2CStudentWelcome = TemplatePrefix() + "B2CStudentWelcome";

        public static readonly string AlstonChildAccountLinked = TemplatePrefix() + "ChildAccountLinked";
        public static readonly string ChildAccountLinked = TemplatePrefix() + "ChildAccountLinked";
        public static readonly string ChildAccountRemoved = TemplatePrefix() + "ChildAccountRemoved";
        public static string AlstonChildAccountRemoved = TemplatePrefix() + "ChildAccountRemoved";

        public static string AlstonGeneralEmail = TemplatePrefix() + "General";
    }

    public static class SuggestionType
    {
        public const string SubjectLevel = "Subject";
        public const string ChapterLevel = "Chapter";
    }

    public const string NA = "N/A";

    public enum UserNotificationTypes
    {
        PackagePurchased = 1,
        SubjectExtend = 2,
        SubjectExpired = 3,
        SubjectExpireSoon = 4,
        SubjectUpgrade = 5,
        SubjectPurchased = 6,
        GetOneAnswer = 7,
        TestPaper = 8,
        GetOneAnswerTcUnlock = 9,
        GenerateTestPaper = 10,
        EditTestPaper = 11,
        AcademicSupport = 12,
        TopUpTutorCredit = 13,
        DeductTutorCredit = 14,
        Task = 15
    }

    public static class RedisKeys
    {
        public static string Profile(string value) => $"Static_Profile:{value}";
        public const string Package = "Static_PACKAGE_KEY";
        public const string Api = "Static_API_KEY";
        public static string DeletedUser(string value) => $"Static_Deleted_User:{value}";
        public static string InactiveUser(string value) => $"Static_Inactive_User:{value}";
    }

    public static string[]? WhitelistedDomains = Array.Empty<string>();
    public static string StaticPrivacyPolicyUrl = "/privacy-policy";
    public static string StaticTermsAndConditionsUrl = "/terms-and-condition";


    public static readonly List<string> XssIgnoredCommandList =
    [
        "AddPackageCallBackCommand",
        "SaveTestPaperSubmissionDetailCommand",
        "AddVideoCallBackCommand",
        "AuthenticationCommand",
        "ValidateForgotPasswordQuery",
        "ResendOtpCommand",
        "ValidateOtpCommand",
        "CreateSuggestionCommand",
        "UpdateSuggestionCommand",
        "VideoTopicRevisionNoteCommand"
    ];

    public static class CustomClaimType
    {
        public const string Email = "Email";
        public const string ForgotPasswordToken = "ForgotPasswordToken";
        public const string UserId = "UserId";
    }

    public static class TokenPurpose
    {
        public const string PasswordResetToken = "ResetPassword";
    }

    public const string AuthorizedAccessToken = "access_token";

    public enum EbookAccessType
    {
        Student = 0,
        Teacher = 1,
        StudentTeacher = 2
    }

    public static class CouponStatusType
    {
        public const string Available = "Available";
        public const string Used = "Used";
        public const string Expired = "Expired";
    }

    public const string StaticCoupon = "Static";

    public static class Cookies
    {
        public const string AccessToken = "static-access-token";
        public const string RefreshToken = "static-refresh-token";
    }

    public static class ClassSuffix
    {
        public const string FreemiumClassSuffix = "_Freemium_Class";
        public const string PremiumClassSuffix = "_Premium_Class";
    }

    public enum SchoolMode
    {
        FullVersion = 0,
        LiteVersion = 1,
        MoeVersion = 2,
        CorporateVersion = 3,
        B2CVersion = 4
    }

    public static class EbookClassSuffix
    {
        public const string EBookFreemiumClassSuffix = "_EBook_Freemium_Class";
        public const string EBookPremiumClassSuffix = "_EBook_Premium_Class";
    }
    
    public const string ErrorMessageForNickName = "Nickname should not contain special characters.";
    public const string ErrorMessageForName = "Name should not contain special characters.";
    public const string ErrorMessageForStatus = "Invalid status.";
    public const string ErrorMessageForDuplicateNickName = "NickName already exists.";

    public static class MstFork
    {
        public const string Nepal = "nepal";
        public const string Indonesia = "indonesia";
        public const string Mce = "mce";
        public const string Alston = "alston";
        public const string Global = "global";
        public const string Thailand = "thailand";
    }

    public static class Language
    {
        public const string English = "en";
        public const string Indonesia = "id";
        public const string UsEnglish = "en-US,en";
        public const string French = "fr";
    }

    public static readonly string[] InnovateTechDomains =
        ["@innovatetech.co", "@innovatetech.io", "@innovatetech.com.np"];

    public enum ClassType
    {
        B2B = 1,
        B2C = 2
    }

    public const string NaSource = "NA";

    public enum AssetType
    {
        Ebook = 0,
        Ivy = 1,
        IvyEbook = 2,
        General = 3
    }

    public enum ClassModel
    {
        Freemium = 0,
        Premium = 1,
        Custom = 2
    }

    public const string FreemiumClassSuffix = "_Freemium_Class";

    public const string PremiumClassSuffix = "_Premium_Class";

    //this is created for random text used in nick name, which will add 4 digit at last.
    public static string RandomNumGenerator()
    {
        var random = new Random();
        return random.Next(1000, 9999).ToString();
    }

    public enum ClassCodeStatus
    {
        Failed = -1,
        NotProcessed = 0,
        Success = 1,
        SelfEnrolledChannelJoinSuccess = 2,
        SelfEnrolledChannelJoinFailed = -2
    }


    public enum ViewSource
    {
        Web = 1,
        Android = 2,
        IOS = 3
    }

    public enum ViewMode
    {
        SelfAssess = 1,
        Teaching = 2
    }

    public enum HeaderType
    {
        Video = 1,
        Interaction = 2,
        Report = 3,
    }

    public enum ActionType
    {
        Create = 1,
        Update = 2,
        Delete = 3,
    }

    public enum FeatureType
    {
        ChatroomFeature = 0,
        SubjectFeature = 1,
        TestPaperFeature = 2,
        ProfilePageFeature = 3,
        ClassroomFeature = 4,
        TeachingResourceFeature = 5,
        TimeLineFeature = 6
    }

    public enum BoSchoolMode
    {
        FullVersion = 0,
        LiteVersion = 1,
        MoeVersion = 2,
    }

    public static class RequestContentType
    {
        public const string Grpc = "application/grpc";
    }

    public static string GetParentThemeForIvy() =>
        BizEnv == StaticConstants.MstFork.Indonesia ? "jelajah" : MstFork.Global;

    public static class AlstonEmailUrl
    {
        public const string MstUrl = "https://app.alstondigital.sg/";
        public const string InstagramUrl = "https://www.instagram.com/alston_digital/";
        public const string FacebookUrl = "https://www.facebook.com/profile.php?id=61569838136874";
        public const string YoutubeUrl = "https://www.youtube.com/@AlstonDigital";
        public const string AppleStoreUrl = "https://apps.apple.com/us/app/alston-digital/id1599207325";
        public const string PlayStoreUrl = "https://play.google.com/store/apps/details?id=com.mysecondteacher.alston&pcampaignid=web_share";
    }

    public static class EmailSubjects
    {
        public const string UserDeactivatedSubject = "Account Deactivated";
        public const string UserActivatedSubject = "Account Activated";
    }

    public static class EmailMessages
    {
        public const string UserDeactivatedMail =
            "Your account has been deactivated from MySecondTeacher. Please contact support@adpeda.com";

        public const string UserActivatedMail = "Your account has been activated for MySecondTeacher.";

        public const string NepalUserActivatedMail =
            "Your mySecondTeacher account has been activated.<br><br>If you believe this is an error or if you have any concerns, please contact through mail, chat or directly call us at +977-9801010144, +977-9801010155.";
    }


    public static class MasteryStatus
    {
        public const string Mastered = "Mastered"; //  100
        public const string AdequateMastery = "Adequate Mastery"; // >= 90 and < 100 

        public const string InadequateMastery = "Inadequate Mastery"; // >=0 <90
        public const string NA = StaticConstants.NA;
    }

    public static class Package
    {
        public const int DefaultOrder = 0;
        public const int ParentOrder = 1;
        public const int PackageTermDefaultOrder = 1;
    }

    #region Session and Attendance Related

    public enum SessionType
    {
        Internal = 0,
        External = 1,
        InClass = 3
    }

    public enum ParticipantType
    {
        Student = 0,
        Class = 1
    }

    public enum AttendanceStatus
    {
        Present = 1,
        Absent = 2,
        Sick = 3,
        Permit = 4
    }

    #endregion

    #region TestPaper Related

    public static class UserTestPaperHistoryStatus
    {
        public const string Draft = "DRAFT";
    }

    public static class TestPaperType
    {
        public const string SelfMarking = "selfMark";
        public const string Normal = "normal";
        public const string Exampaper = "exampaper";
    }

    public const int DefaultMaxCustomTestPaperQuestions = 15;

    public static class TestPaperBankType
    {
        public const string Custom = "custom";
        public const string Standard = "standard";
        public const string PreDefined = "predefined";
        public const string Competency = "competency";
        public const string Exampaper = "exampaper";
    }

    public static class GrpcMetadata
    {
        public const string Authorization = "Authorization";
        public const string UserAgent = "user-agent";
    }

    public enum CompetencyTestPaperType
    {
        Pre = 1,
        Post = 2
    }

    public enum CompetencyTestPaperSubmissionHistoryStatus
    {
        InProgress = 1,
        Completed = 2,
    }

    public enum BucketTag
    {
        Teacher = 0, //"TEACHER";
        General = 1, //"GENERAL";
    }

    #endregion

    #region User Related
    
    public enum Gender
    {
        Female = 1,
        Male = 2,
        Others = 3
    }

    // public enum UserRole
    // {
    //     SuperAdmin = 0,
    //     Parent = 1,
    //     Student = 2,
    //     SchoolLeader = 3,
    //     BoAdmin = 4,
    //     BoLiteAdmin = 5,
    //     ApTutor = 6,
    //     CSR = 7,
    //     HQLeader = 8,
    //     Onboarder = 9,
    //     Teacher = 10,
    //     PUBLISHERADMIN =11
    // }

    public enum UserMode
    {
        FullVersion = 0
    }

    public static readonly string[] UserRoles = Enum.GetNames<StaticRole>();

    public static readonly StaticRole[] AdminRoles = [StaticRole.SuperAdmin];
    
    #endregion

    #region UserTestPaperLog

    public class UserTestPaperLogRemark
    {
        public static readonly string SelfGenerated = "Self-grading TestPaper generated.";
        public static readonly string ExamPaperGenerated = "ExamPaper generated.";

        public static readonly string DuplicatedAlongAllQuestion = "TestPaper duplicated along with all questions.";
        public static string CustomGenerated(string testPaperType) => $"Custom {testPaperType} TestPaper generated.";
        public static string CreatedByAdmin(string adminName) => $"TestPaper balance created by admin {adminName}.";
        public static string DeductedByAdmin(string adminName) => $"TestPaper balance deducted by admin {adminName}.";

        public static string CreatedAgainstPackage(string packageName) =>
            $"TestPaper balance credited for the purchase of {packageName} package.";

        public static string AddedForRegistration(decimal balance) =>
            $"{balance} test paper balance added for registration.";
    }

    public class UserTutorCreditLogRemark
    {
        public static string CreatedAgainstPackage(string packageName) =>
            $"Tutor Credit Added for the purchase of {packageName} package.";
    }

    #endregion

    #region Amazon

    public static Amazon.RegionEndpoint AwsRegionEndpoint =
        !IsNotProduction
            ? (BizEnv == "nepal" && DeploymentEnv == "nepal-production") ? Amazon.RegionEndpoint.APSouth1 :
            (BizEnv == "indonesia") ? Amazon.RegionEndpoint.APSoutheast3 :
            Amazon.RegionEndpoint.APSoutheast1
            : Amazon.RegionEndpoint.APSoutheast1;

    public static Amazon.RegionEndpoint AwsTeacherContentBucketEndPoint =
        !IsNotProduction
            ? (BizEnv == "nepal" && DeploymentEnv == "nepal-production") ? Amazon.RegionEndpoint.APSouth1 :
            (BizEnv == "indonesia") ? Amazon.RegionEndpoint.APSoutheast3 :
            Amazon.RegionEndpoint.APSoutheast1
            : Amazon.RegionEndpoint.APSoutheast1;

    public static Amazon.RegionEndpoint AwsRegionClassroomBucketEndPoint =
        !IsNotProduction
            ? (BizEnv == "nepal" && DeploymentEnv == "nepal-production")
                ? Amazon.RegionEndpoint.APSouth1
                : Amazon.RegionEndpoint.APSoutheast1
            : Amazon.RegionEndpoint.APSoutheast1;

    public static Amazon.RegionEndpoint AwsRegionSpecialLaunchPageBucketEndpoint =
        !IsNotProduction
            ? (BizEnv == "nepal" && DeploymentEnv == "nepal-production")
                ? Amazon.RegionEndpoint.APSouth1
                : Amazon.RegionEndpoint.APSoutheast1
            : Amazon.RegionEndpoint.APSoutheast1;

    public static Amazon.RegionEndpoint AwsRegionAttachmentsBucketEndpoint =
        !IsNotProduction
            ? (BizEnv == "nepal" && DeploymentEnv == "nepal-production") ? Amazon.RegionEndpoint.APSouth1 :
            (BizEnv == "indonesia") ? Amazon.RegionEndpoint.APSoutheast3 :
            Amazon.RegionEndpoint.APSoutheast1
            : Amazon.RegionEndpoint.APSoutheast1;

    public static Amazon.RegionEndpoint AwsClassroomZipLambdaEndpoint =
        !IsNotProduction
            ? (BizEnv == "nepal" && DeploymentEnv == "nepal-production")
                ? Amazon.RegionEndpoint.APSouth1
                : Amazon.RegionEndpoint.APSoutheast1
            : Amazon.RegionEndpoint.APSoutheast1;

    public static Amazon.RegionEndpoint GetAwsRegionEndpoint(string invokedFrom)
    {
        // S3 Assignment upload 

        if (BizEnv == "indonesia" && (invokedFrom == "Assignment" || invokedFrom == "SNS"))
        {
            return Amazon.RegionEndpoint.APSoutheast1;
        }

        return AwsRegionEndpoint;
    }

    #endregion

    public enum PublishingStatus
    {
        Draft = 1,
        Published = 2,
        Deleted = 3,
        Deactivated = 4
    }


    public static class PaginationDefaults
    {
        // default = No pagination
        public const int PageNumber = 1;
        public const int PageSize = 8;


        public const int MinPageSize = 1;
        public const int MinPageNumber = 1;
    }

    public static class HttpClients
    {
        public const string CloudFlare = "CloudFlare";
        public const string EventingSystem = "EventingSystem";
        public const string Reporting = "Reporting";
        public const string PlantUml = "PlantUml";
    }

    #region AWS

    public static class AWSFolderConfigurations
    {
        public const string ImagesFolder = "images";
        public const string ResourcesFolder = "resources";
        public const string AnnouncementFilesFolder = "announcements";
        public const string UserContentFolder = "userContent";
        public const string AcademicSupportAttachmentFolder = "";
        public const string EbookAttachmentFolder = "";
        public const string TaskFolder = "";
    }

    #endregion


    public static class EsewaResponseCode
    {
        public const string Failure = "failure";
        public const string Success = "Success";
        public const string Complete = "COMPLETE";
    }

    public enum BulkAuditStatus
    {
        Pending = -1,
        Failed = 0,
        Success = 1,
        InProgress = 2
    }

    public enum AnnouncementType
    {
        General = 1, //Make General Announcement
        Consent = 2 //Get Response (e.g. Send eConsent Forms)
    }

    public enum AnnouncementToGroup
    {
        Class = 1, // Announcement message to specific class(es) => TEACHER,INTEL
        School = 2, //Announcement message to entire school => INTEL
        TeacherClass = 3, //Announce message to all classes that you teach => TEACHER
        Level = 4 //Announcement message to particular level(s) => INTEL
    }
    
    public static class AnnouncementConsentResponseErrorMessage
    {
        public const string ExpiredErrorMessage =
            "Cannot submit the response! The announcement has already expired!";

        public const string ExpiredIndoErrorMessage =
            "Tidak dapat mengirimkan tanggapan! Pengumuman telah kedaluwarsa!";
    }


    public static class XDevice
    {
        public const string MstTv = "mst-TV";
        public const string MstMobile = "mst-Mobile";
        public const string MstEndUser = "mst-EndUser";
        public const string MstBackOffice = "mst-BackOffice";
    }

    public static class AnnouncementReceivers
    {
        public const string Parents = "Parents";
        public const string Teachers = "Teachers";
        public const string Students = "Students";
    }

    public static class Attachments
    {
        public const string ChatroomPath = "resources/attachments/group-chat";
        public const string ClassroomPath = "resources/attachments/classroom";
        public const string SessionPath = "resources/attachments/session";
        public const string AnnouncementPath = "resources/attachments/announcement";
    }
    
    public static class HttpHeaders
    {
        public const string Device = "x-device";
        public const string Bypass2Fa = "x-bypass-2fa";
        public const string DeviceType = "x-device-type";
        public const string AppVersion = "x-app-version";
        public const string OldAppVersion = "app_version";
        public const string AppVersionCode = "x-app-version-code";
        public const string DeviceToken = "x-device-token";
        public const string DeviceId = "x-device-id";
    }
    
    public static class PasswordRules
    {
        public const int MinLength = 8;
        public const string UpperCasePattern = "[A-Z]";
        public const string LowerCasePattern = "[a-z]";
        public const string DigitPattern = "[0-9]";
        public const string SpecialCharPattern = "[!@#$%^&*(),.?\":{}|<>]";
    }
    
    public static class BulkUpdateErrorMessage
    {
        public const string EmptyEmail = "Email is empty";
        public const string EmptyActivation = "Activation is empty";
        public const string InvalidEmailAddress = "Invalid Email Address";
        public const string InvalidActivationValue = "Invalid Activation Value";
        
        public const string NotInSchoolMessage = "The student email does not exists in your current school";
        public const string FileUploadError = "Sorry, the file was not uploaded. Please try again.";
        public const string InvalidCsvFileMessage = "Invalid CSV file";
        public const string EmptyCsvFileMessage = "Empty CSV file";
        public const string DeActiveMessageTemplate = "Your account has been deactivated from MySecondTeacher.";
        public const string NepalDeActiveMessageTemplate = "Your mySecondTeacher account has been deactivated.<br><br>If you believe this is an error or if you have any concerns, please contact through mail, chat or directly call us at +977-9801010144, +977-9801010155.";
    }
    
    public static class BulkUpdateMessage
    {
        public const string SuccessMessage = "Student status updated successfully";

        public const string NoEmailMessage = "Cannot update the student status becuase student email doesnot exist";

        public const string AlreadyActiveMessage = "Student is already Active";
        public const string AlreadyInactiveMessage = "Student is already InActive";

        public const string ActiveMessageTemplate =
            "Your account has been activated at MySecondTeacher. {0} Please login at {1} to start learning!";
        public const string NepalActiveMessageTemplate =
            "We're pleased to inform you that your mySecondTeacher account has been successfully activated by your school.<br><br>You can now access all features and resources available on the platform.";
    }

    public enum SubscriptionPlanType
    {
        Monthly = 1,
        HalfYearly = 6,
        Yearly = 12
    }
    public static class EbookOfflineReporting
    {
        public enum ReportingSourceType
        {
            Web = 1,
            Mobile = 2,
            Offline = 3
        }

        public enum ReportingOfflineAction
        {
            Insert = 1,
            Update = 2,
            Delete = 3
        }

        public static class OfflineReportingTypes
        {
            public const string Bookmark = "bookmark";
            public const string Note = "note";
            public const string Scribble = "scribble";
        }
    }
    
    public static class CustomPaymentRemarks
    {
        public const string Esewa = "Payment initaited through eSewa";
        public const string Khalti = "Payment initaited through Khalti";
        public const string FonePay = "Payment initaited through FonePay";
        public const string Stripe = "Payment initaited through Stripe";
        public const string PayPal = "Payment initaited through PayPal";
    }

    public enum PreSignedUrlType
    {
        DefaultFile = 1,
        TeacherContent = 2,
        Classroom = 3,
        SpecialLaunchPage = 4,
        ClassroomZip = 5
    }

    public enum FileUploadInvokedFrom
    {
        DefaultFile = 1,
        SpecialLaunchPage = 2,
        ImageFile = 3
    }

    public enum ServiceType
    {
        Normal = 1,
        TestPaper = 2,
        NormalTestPaper =3,
        Ivy = 4,
        Ebook =5,
        CompetencyTestPaper =6,
        Kfq= 7,
        InBookQuiz = 8
    }

    public enum SubmissionStatus
    {
        Submitted = 1,
        Graded = 2,
        Pending = 3
    }
    
    public const string InvalidFileCharacters = @"[<>:""/\\|?*']";
    public const string LinkAttachment = "Link";
    public const string FileAttachment = "File";
    public const string NormalTestpaperAsset = "NormalTestpaper";
    public const string NormalAsset = "Normal";
    public const string AssignmentGraded = "Graded";
    public const string AssignmentSubmitted = "Submitted";
    public const string AssignmentPending = "Pending";
    public const string AssignmentMissing = "Missed";
    
    public static class SourceType
    {
        public const int AddAttachment = 1;
        public const int RemoveAttachment = 2;
        public const int SubmitAttachment = 3;
        public const int RemarkAttachment = 4;
    }

    
    public enum ClassroomReportTypes
    {
        Session = 0,
        Assignment = 1,
    }
}