<<<<<<< HEAD
﻿namespace Examination_System.Common.Wrappers
=======
namespace Examination_System.Common.Wrappers
>>>>>>> origin/Test
{
    public enum ErrorCode
    {
        None=0,

        //Diploma
<<<<<<< HEAD
        DiplomaNotFound = 101,
        EmptyDiplomaArray = 102,
        InvaildDiplomaId = 103,
        DiplomaIsNotActive = 104,
        NoParamSent = 105,

        //Quiz
        QuizNotFound = 201,
        QuizTitleExists = 202,
        InvalidQuizData = 203,

        //attempt
        AttemptNotFound = 301,
        AttemptClosed = 302,
        AttemptExpired = 303,
        Forbidden = 304,
        AttemptInProgress = 305,
        AttemptLimitReached = 306,
        //Option
        InvalidOption = 401,

        //Question
        InvalidQuestion = 501,

            //Auth
            InvalidCredentials = 601,
            UserNotFound = 602,
            EmailAlreadyExists = 603,
            UsernameAlreadyExists = 604,
            EmailNotConfirmed = 605,
            InvalidOtp = 606,
            OtpExpired = 607,
            OtpAlreadyUsed = 608,
            TooManyOtpRequests = 609,
            InvalidResetToken = 610,
            ResetTokenExpired = 611,
            PasswordTooWeak = 612,
            UnauthorizedAccess = 613,
            AccountLocked = 614,
            TooManyLoginAttempts = 615,
            InvalidOperation = 616,
            OperationFailed = 617,




            // Other Errors
            NotFound = 1,
            BadRequest = 3,
            Conflict = 4,
            UnprocessableEntity = 5,
            InternalServerError = 6,
            Unauthorized = 7,
            ValidationError = 8,
            TooManyRequests = 9,
       
=======
             DiplomaNotFound=101,
             EmptyDiplomaArray=102,
             InvaildDiplomaId = 103,
             DiplomaIsNotActive = 104,
             NoParamSent = 105,

//Quiz
              QuizNotFound = 201,
              QuizTitleExists = 202,
              InvalidQuizData = 203,

         //attempt
             AttemptNotFound = 301,
             AttemptClosed = 302,
             AttemptExpired = 303,
             Forbidden = 304,
             AttemptInProgress = 305,
             AttemptLimitReached = 306,
        //Option
             InvalidOption=401,

        //Question
             InvalidQuestion= 501
>>>>>>> origin/Test

    }
}
