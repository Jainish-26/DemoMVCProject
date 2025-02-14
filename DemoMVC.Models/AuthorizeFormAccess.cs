using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Model
{
     public class AuthorizeFormAccess
    {
        public enum FormAccessCode
        {
            CANDIDATE = 1,
            //CVVALIDATE = 2,
            //SENDCV = 3,
            //CUSTOMER = 4,
            //PROJECT = 5,
            //CXREP = 6,
           
            //LANGUAGE = 8,
            //COUNTRY = 9,
            FORMMASTER = 10,
            ROLES = 11,
            //UPDATESTS = 12,
            //MANGINITFEED = 13,
            //MANGSENTCVS = 14,
            EMAILTEMPLATE = 15,
            //SENDINTREQ = 16,
            USER = 17,
            REQUIREMENTS = 18,
            //CPSCREEN = 19,
            COMMONLOOKUP = 20,
            STATUS = 21,
            //SCHEDULEINTR = 22,
            //SENDCALINVITE = 23,
            EMAILLOG = 24,
            DASHBOARD = 25,
            //UPDATEFEEDBACK = 25,
            //INTRREMTOCAND = 26,
            //SENDFBREMINDERS = 27,
            //SENDINTIFBREMINDERS = 28,
            //UPDATEFEEDBACKALL = 29,
            //SCHEDULEINTERVIEWALL = 30,
            //SENDCALENDARINVITEALL = 31,
            //SENDFEEDBACKREMINDERALL = 32,
            //UPDATESTATUSALL = 33,
            //SENDINTERVIEWREMTOCANDIDATEALL = 34,
            //SENDINTIFEEDBACKREMINDERALL = 35,
            //UPDATEINTERVIEWFEEDBACKALL = 36,
            //UPDATEINTERVIEWFEEDBACK = 37,
            //DASHBOARD = 38,
            //OFFERFOLLOWUP = 39,
            //OFFERFOLLOWUPALL = 40,
            //REPORTS = 41,
            //HIRINGTRACKERREPORT = 42,
            //ONBOARDINGTRACKERREPORT = 43,
            //SENDOFFERLETTER = 44,
            //BGV = 45,
            //VENDOR = 46,
            //SENDDOCUMENTSTOVENDOR = 47,
            //SENDPHOTO = 48,
            POSITION =49,
            INTERVIEWER=50,
            ORGANIZATION=54,
            CANDIDATEREQUIREMENTMAPPING=51,
            SENDMAILTODECIDINGAUTHORITY=52,
            SCHEDULEINTERVIEW=53,
            SENDCALENDARINVITE=55,
            ERRORLOG = 56,
            ACTIVITYLOG =57,
            MESSAGE = 58,
            UPDATESTATUS = 59,
            INTERESTEDCANDIDATES=60,
            SENDINTERVIEWREMINDER=61,
            JOBLOCATION = 62
        }
    }
}
