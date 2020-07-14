using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Mvc;
using System.Text.RegularExpressions;

using Driven.Business.Royalty;
using Driven.Business.Royalty.Interfaces;
using Driven.Data.Entity.Royalty;

using Driven.App.BrandPeriodSalesReporting.Models;
using Driven.App.BrandPeriodSalesReporting.Helpers;


namespace Driven.App.BrandPeriodSalesReporting.Controllers
{
    [BpsrAuthorize(Roles = BpsrRoles.Admin)]
    public class DenialTypeController : BaseController
    {
        private const string mc_ExceptionMessage_Error = "Something went wrong.";
        private const string mc_ExceptionMessage_NoAccess = "You are not allowed to perform this operation.";

        public static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private IDenialTypeService _denialTypeService;

        public DenialTypeController() : this(new DenialTypeService()) { }

        public DenialTypeController(IDenialTypeService denialTypeService)
        {
            _denialTypeService = denialTypeService;
        }


        public ActionResult Index()
        {
            var model = new DenialTypeListViewModel();
            return View(model);
        }


        [HttpGet]
        public ActionResult GetDenialTypes()
        {
            var model = new List<DenialTypesViewModel>();
            var denialTypes = _denialTypeService.GetDenialTypes().ToList();

            foreach (var f in denialTypes)
            {
                model.Add(new DenialTypesViewModel() { DenialTypeId = f.BPSR_DenialTypeID, DenialTypeCodeId = f.DenialTypeCode, DenialTypeDescription = f.DenialTypeDescription });
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult CreateDenialType()
        {
            try
            {
                var model = new CreateDenialTypeViewModel();
                return PartialView("_CreateDenialType", model);
            }
            catch (Exception ex)
            {
                _log.Error(FormatException(ex));
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return Json(mc_ExceptionMessage_Error, JsonRequestBehavior.AllowGet);
            }
        }


        private bool ValidateDenialTypeId(string id)
        {
            int denialTypeId = 0;
            if (!Int32.TryParse(id, out denialTypeId))
            {
                return false;
            }
            else
            {
                return (_denialTypeService.GetDenialTypes().Where(x => x.BPSR_DenialTypeID == denialTypeId).FirstOrDefault() == null);
            }
        }


        private bool ValidateDenialTypeCode(string code)
        {
            return (_denialTypeService.GetDenialTypes().Where(x => x.DenialTypeCode == code).FirstOrDefault() == null);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDenialType(CreateDenialTypeViewModel model)
        {
            var success = false;
            var message = mc_ExceptionMessage_Error;

            try
            {
                if (!ValidateDenialTypeId(model.DenialTypeId))
                {
                    ModelState.AddModelError("DenialTypeId", "The Denial Type Id must be unique.");
                }

                if (!ValidateDenialTypeCode(model.DenialTypeCodeId))
                {
                    ModelState.AddModelError("DenialTypeCodeId", "The Denial Type Code must be unique.");
                }

                if (ModelState.IsValid)
                {
                    _denialTypeService.CreateNewDenialType(Convert.ToInt32(model.DenialTypeId), model.DenialTypeCodeId, model.DenialTypeDescription);
                    success = true;
                    message = "Successfully added Denial Type.";
                }
            }
            catch (Exception ex)
            {
                _log.Error(FormatException(ex));
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return Json(message, JsonRequestBehavior.AllowGet);
            }

            ViewBag.DialogResult = BuildDialogResult(success, message);
            return PartialView("_CreateDenialType", model);
        }


        [HttpGet]
        public ActionResult EditDenialType(int denialTypeId)
        {
            try
            {
                var denialType = _denialTypeService.GetDenialTypeById(denialTypeId);
                if (denialType == null)
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                    return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
                }

                var model = new EditDenialTypeViewModel() { DenialTypeId = denialType.BPSR_DenialTypeID, DenialTypeCodeId = denialType.DenialTypeCode, DenialTypeDescription = denialType.DenialTypeDescription };
                return PartialView("_EditDenialType", model);
            }
            catch (Exception ex)
            {
                _log.Error(FormatException(ex));
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return Json(mc_ExceptionMessage_Error, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDenialType(EditDenialTypeViewModel model)
        {
            var success = false;
            var message = mc_ExceptionMessage_Error;

            try
            {
                var denialType = _denialTypeService.GetDenialTypeById(model.DenialTypeId);

                if (denialType == null)
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                    return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
                }

                if (ModelState.IsValid)
                {
                    _denialTypeService.UpdateDenialType(denialType.BPSR_DenialTypeID, denialType.DenialTypeCode, model.DenialTypeDescription);
                    success = true;
                    message = "Successfully updated Denial Type.";
                }
            }
            catch (Exception ex)
            {
                _log.Error(FormatException(ex));
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return Json(mc_ExceptionMessage_Error, JsonRequestBehavior.AllowGet);
            }

            ViewBag.DialogResult = BuildDialogResult(success, message);
            return PartialView("_EditDenialType", model);
        }


    }
}
