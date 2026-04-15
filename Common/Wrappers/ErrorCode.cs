namespace Examination_System.Common.Wrappers
{
    public enum ErrorCode
    {
        None=0,

        //Diploma
             DiplomaNotFound=101,
             EmptyDiplomaArray=102,
             InvaildDiplomaId = 103,
             DiplomaIsNotActive = 104,
             NoParamSent = 105,

         //Quiz
             QuizNotFound = 201,

         //attempt
             AttemptNotFound = 301,
             AttemptClosed = 302,
             AttemptExpired=303,
             Forbidden= 304,
        //Option
             InvalidOption=401,

        //Question
             InvalidQuestion= 501

    }
}
