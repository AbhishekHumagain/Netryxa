namespace Static.Application.Common.Interfaces
{
    public interface IAwsConfiguration
    {
        string AccessKey { get; set; }
        string SecretKey { get; set; }
        string EmailSqsUrl { get; set; }
        string AssetsS3BucketName { get; set; }
        string AcademicSupportAttachmentFolder { get; set; }
        string ImagesFolder { get; set; }
        string WebSocketSnsTopicArn { get; set; }
        string EbookAttachmentFolder { get; set; }
        string VideoTopicRevisionNoteFolder { get; set; }
        string TeachingGuideAttachmentFolder { get; set; }
        string UserContentFolder { get; set; }
        string ResourcesFolder { get; set; }
        string TeachingResourcesSqs { get; set; }
        string TaskFolder { get; set; }
        string SpecialLaunchPageAttachmentsBucketFolder { get; set; }
        string AttachmentsBucket { get; set; }
        string AssignPremiumPackageFunctionName { get; set; }
        string UserNotificationLambdaQueue { get; set; }
        string AnnouncementAttachmentsBucketFolder { get; set; }
    }
}
