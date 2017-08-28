using EMPPortal.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EMPPortal.Controllers
{
    [SessionCheck]
    public class BankEnrollmentController : Controller
    {
        // GET: Bank Selection
        public ActionResult Index()
        {
            return View();
        }

        // GET: Bank Selection Fee Details
        public ActionResult BankSelectionFeeDetails()
        {
            return View();
        }

        // GET: Bank Fee Selection
        public ActionResult BankFeeSelection()
        {
            return View();
        }

        // GET: Fee Reimbursements
        public ActionResult FeeReimbursements()
        {
            return View();
        }

        // GET: Bank Enrollment TPG
        public ActionResult BankEnrollmentTPG()
        {
            return View();
        }

        // GET: Bank Enrollment Refund Advantage
        public ActionResult BankEnrollmentRA()
        {
            return View();
        }

        // GET: Bank Enrollment Republic Bank   
        public ActionResult BankEnrollmentRB()
        {
            return View();
        }
        // GET:e-Filing Payment Options   
        public ActionResult eFillingPaymentOption()
        {
            return View();
        }
        // GET: e-Filing Credit Card   
        public ActionResult eFillingcreditCard()
        {
            return View();
        }
        // GET: e-File ACH 
        public ActionResult eFileACH()
        {
            return View();
        }
        // GET: Balance Due on Account
        public ActionResult BalanceDueonAccount()
        {
            return View();
        }
        // GET: Balance Due on Account Credit Card
        public ActionResult BalanceDueonAccountCreditCard()
        {
            return View();
        }
        // GET: Balance Due on Account ACH
        public ActionResult BalanceDueonAccountACH()
        {
            return View();
        }
        // GET: Data Table
        public ActionResult DataTable()
        {
            return View();
        }
    }
}