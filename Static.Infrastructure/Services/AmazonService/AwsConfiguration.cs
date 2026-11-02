using Static.Application.Common.Interfaces;
using Static.Helper.Constants;

namespace Static.Infrastructure.Services.AmazonService
{
    public class AwsConfiguration : IAwsConfiguration
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string EmailSqsUrl { get; set; }
        public string AssetsS3BucketName { get; set; }
        public string AcademicSupportAttachmentFolder { get; set; }
        public string ImagesFolder { get; set; }
        public string EbookAttachmentFolder { get; set; }
        public string VideoTopicRevisionNoteFolder { get; set; }
        public string WebSocketSnsTopicArn { get; set; }
        public string TeachingGuideAttachmentFolder { get; set; }
        public string UserContentFolder { get; set; } = StaticConstants.AWSFolderConfigurations.UserContentFolder;
        public string ResourcesFolder { get; set; }
        public string TeachingResourcesSqs { get; set; }
        public string TaskFolder { get; set; }
        public string AssignPremiumPackageFunctionName { get; set; }
        public string UserNotificationLambdaQueue { get; set; }
        public string AnnouncementAttachmentsBucketFolder { get; set; }
        public string SpecialLaunchPageAttachmentsBucketFolder { get; set; }
        public string AttachmentsBucket { get; set; }
    }
}