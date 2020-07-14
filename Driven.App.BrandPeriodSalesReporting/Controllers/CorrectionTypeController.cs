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
    public class CorrectionTypeController : BaseController
    {
        private const string mc_ExceptionMessage_Error = "Something went wrong.";
        private const string mc_ExceptionMessage_NoAccess = "You are not allowed to perform this operation.";

        public static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private ICorrectionTypeService _correctionTypeService;

        public CorrectionTypeController() : this(new CorrectionTypeService()) { }

        public CorrectionTypeController(ICorrectionTypeService correctionTypeService)
        {
            _correctionTypeService = correctionTypeService;
        }


        public ActionResult Index()
        {
            var model = new CorrectionTypeListViewModel();
            return View(model);
        }


        [HttpGet]
        public ActionResult GetCorrectionTypes()
        {
            var model = new List<CorrectionTypesViewModel>();
            var correctionTypes = _correctionTypeService.GetCorrectionTypes().ToList();

            foreach (var f in correctionTypes)
            {
                model.Add(new CorrectionTypesViewModel() { CorrectionTypeId = f.BPSR_CorrectionTypeID, CorrectionTypeCodeId = f.CorrectionTypeCode, CorrectionTypeDescription = f.CorrectionTypeDescription });
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult CreateCorrectionType()
        {
            try
            {
                var model = new CreateCorrectionTypeViewModel();
                return PartialView("_CreateCorrectionType", model);
            }
            catch (Exception ex)
            {
                _log.Error(FormatException(ex));
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return Json(mc_ExceptionMessage_Error, JsonRequestBehavior.AllowGet);
            }
        }


        private bool ValidateCorrectionTypeId(string id)
        {
            int correctionTypeId = 0;
            if (!Int32.TryParse(id, out correctionTypeId))
            {
                return false;
            }
            else
            {
                return (_correctionTypeService.GetCorrectionTypes().Where(x => x.BPSR_CorrectionTypeID == correctionTypeId).FirstOrDefault() == null);
            }
        }

        
        private bool ValidateCorrectionTypeCode(string code)
        {
            return (_correctionTypeService.GetCorrectionTypes().Where(x => x.CorrectionTypeCode == code).FirstOrDefault() == null);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCorrectionType(CreateCorrectionTypeViewModel model)
        {
            var success = false;
            var message = mc_ExceptionMessage_Error;

            try
            {
                if (!ValidateCorrectionTypeId(model.CorrectionTypeId))
                {
                    ModelState.AddModelError("CorrectionTypeId", "The Correction Type Id must be unique.");
                }

                if (!ValidateCorrectionTypeCode(model.CorrectionTypeCodeId))
                {
                    ModelState.AddModelError("CorrectionTypeCodeId", "The Correction Type Code must be unique.");
                }
                
                if (ModelState.IsValid)
                {
                    _correctionTypeService.CreateNewCorrectionType(Convert.ToInt32(model.CorrectionTypeId), model.CorrectionTypeCodeId, model.CorrectionTypeDescription);
                    success = true;
                    message = "Successfully added Correction Type.";
                }
            }
            catch (Exception ex)
            {
                _log.Error(FormatException(ex));
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return Json(message, JsonRequestBehavior.AllowGet);
            }

            ViewBag.DialogResult = BuildDialogResult(success, message);
            return PartialView("_CreateCorrectionType", model);
        }


        [HttpGet]
        public ActionResult EditCorrectionType(int correctionTypeId)
        {
            try
            {
                var correctionType = _correctionTypeService.GetCorrectionTypeById(correctionTypeId);
                if (correctionType == null)
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                    return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
                }

                var model = new EditCorrectionTypeViewModel() { CorrectionTypeId = correctionType.BPSR_CorrectionTypeID, CorrectionTypeCodeId = correctionType.CorrectionTypeCode, CorrectionTypeDescription = correctionType.CorrectionTypeDescription };
                return PartialView("_EditCorrectionType", model);
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
        public ActionResult EditCorrectionType(EditCorrectionTypeViewModel model)
        {
            var success = false;
            var message = mc_ExceptionMessage_Error;

            try
            {
                var correctionType = _correctionTypeService.GetCorrectionTypeById(model.CorrectionTypeId);
                
                if (correctionType == null)
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                    return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
                }

                if (ModelState.IsValid)
                {
                    _correctionTypeService.UpdateCorrectionType(correctionType.BPSR_CorrectionTypeID, correctionType.CorrectionTypeCode, model.CorrectionTypeDescription);
                    success = true;
                    message = "Successfully updated Correction Type.";
                }
            }
            catch (Exception ex)
            {
                _log.Error(FormatException(ex));
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return Json(mc_ExceptionMessage_Error, JsonRequestBehavior.AllowGet);
            }

            ViewBag.DialogResult = BuildDialogResult(success, message);
            return PartialView("_EditCorrectionType", model);
        }


    }
}
