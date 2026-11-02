using Static.Helper.Constants;
using Static.Application.Common.Interfaces;
using Static.Application.Common.Models;

namespace Static.Application.Common.Notifications;

public static class EmailNotification
{
    public static void CompleteStudentRegistration(this IRabbitMQProducer rabbitMqProducer, string toAddress,
        string fullName, string passwordResetUrl, string activeYear)
    {
        var sendEmailRequest = new EmailQueueRequestDto
        {
            ToAddress = toAddress,
            EmailTemplateName = StaticConstants.EmailTemplate.CompleteStudentRegistration,
            TemplateData = new Dictionary<string, string>
            {
                { "fullName", fullName.TrimStart().TrimEnd() },
                { "passwordResetURL", passwordResetUrl },
                { "registeredSupportEmail", StaticConstants.StaticSupportEmail },
                { "activeYear", activeYear },
                { "privacyPolicyUrl", StaticConstants.StaticPrivacyPolicyUrl },
                { "termsAndConditionsUrl", StaticConstants.StaticTermsAndConditionsUrl }
            }
        };
        rabbitMqProducer.SendEmailToQueue(sendEmailRequest);
    }
    public static void StudentForgotPassword(this IRabbitMQProducer rabbitMqProducer, string toAddress,
        string setPasswordUrl)
    {
        var activeYear = DateTime.UtcNow.Year;
        var sendEmailRequest = new EmailQueueRequestDto
        {
            ToAddress = toAddress,
            EmailTemplateName = StaticConstants.EmailTemplate.StudentForgotPassword,
            TemplateData = new Dictionary<string, string>
            {
                { "fullName", toAddress },
                { "passwordResetURL", setPasswordUrl },
                { "registeredSupportEmail", StaticConstants.StaticSupportEmail },
                { "activeYear", activeYear.ToString() }
            }
        };
        rabbitMqProducer.SendEmailToQueue(sendEmailRequest);
    }
    public static void ParentAndCeForgotPassword(this IRabbitMQProducer rabbitMqProducer, string toAddress,
        string setPasswordUrl)
    {
        var activeYear = DateTime.UtcNow.Year;
        var sendEmailRequest = new EmailQueueRequestDto
        {
            ToAddress = toAddress,
            EmailTemplateName = StaticConstants.EmailTemplate.ParentAndCeForgotPassword,
            TemplateData = new Dictionary<string, string>
            {
                { "fullName", toAddress.TrimStart().TrimEnd() },
                { "passwordResetURL", setPasswordUrl },
                { "registeredSupportEmail", StaticConstants.StaticSupportEmail },
                { "activeYear", activeYear.ToString() }
            }
        };
        rabbitMqProducer.SendEmailToQueue(sendEmailRequest);
    }

    public static void CompleteBoAdminRegistration(this IRabbitMQProducer rabbitMqProducer,
        string toAddress, string fullName, string passwordResetUrl)
    {
        var sendEmailRequest = new EmailQueueRequestDto
        {
            ToAddress = toAddress,
            EmailTemplateName = StaticConstants.EmailTemplate.NonStudentRegistration,
            TemplateData = new Dictionary<string, string>
            {
                { "fullName", fullName }, { "passwordResetURL", passwordResetUrl }
            }
        };
        rabbitMqProducer.SendEmailToQueue(sendEmailRequest);
    }
}