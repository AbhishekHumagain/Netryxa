using Static.Results;
using System.Net;

namespace Static.Domain.Errors;

public class TestPaperError
{
    public static readonly Error UnAbleToFetch = new(HttpStatusCode.NotFound,"Unable to fetch test paper.");
    public static readonly Error TpSectionMax = new(HttpStatusCode.BadRequest,"Maximum number of sections exceeded.!");
    public static readonly Error TpSectionQuestionMax = new(HttpStatusCode.BadRequest,"Maximum number of questions in a section exceeded.");
    public static readonly Error TitleEmpty = new(HttpStatusCode.BadRequest,"Unable to create test paper. Title cannot be empty.");
    public static readonly Error MaxTpGeneratedExceed = new(HttpStatusCode.BadRequest,"Maximum number of TestPaper generation exceeded.");
    public static readonly Error MaxTpQuestionGeneratedExceed = new(HttpStatusCode.BadRequest,"Maximum number of questions exceeded.");
    public static readonly Error LinkedTpSubject = new(HttpStatusCode.NotFound,"Linked TestPaper Subject not found.");
    public static readonly Error LevelNotFound = new(HttpStatusCode.NotFound,"Level Not Found!");
    public static readonly Error TestPaperHistoryNotFound = new(HttpStatusCode.NotFound,"Could not found the test-paper history, maybe it's deleted!");
    public static readonly Error PredefinedNotFound = new(HttpStatusCode.NotFound, "Predefined TestPaper not found!");
    public static readonly Error TpNotFound = new(HttpStatusCode.NotFound, "Test paper not found!");
    public static readonly Error TpNotEditable = new(HttpStatusCode.BadRequest, "Test paper not Editable.");
}