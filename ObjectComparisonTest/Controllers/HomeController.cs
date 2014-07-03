using ObjectComparisonTest.Models;
using ObjectComparisonTest.Service.Interface;
using ObjectComparisonTest.Service.Logging;
using ObjectComparisonTest.Service.ObjectParameters;
using ObjectComparisonTest.Service.ObjectResponse;
using System;
using System.Web.Mvc;

namespace MVCObjectComparison.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        private readonly IComparisonService comparisonService;


        public HomeController(IComparisonService comparisonService)
        {
            this.comparisonService = comparisonService;
        }

        public ActionResult Index()
        {
            
            var model = new HomeModel();

            try
            {
                Logger.Info("Begin model creation");

                model = SetupSomeTypeParameters();

                model.ComparisonResponseForAB = comparisonService.GetChanges(model.ComparisonA, model.ComparisonB);

                model.ComparisonResponseForCD = comparisonService.GetChanges(model.ComparisonC, model.ComparisonD);

                Logger.Info("End model creation");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
            }
            return View(model);
        }


        private HomeModel SetupSomeTypeParameters()
        {
            var HomeModel = new HomeModel() 
            {
                ComparisonA = new SomeTypeParameters()
                {
                    Forename = "David",
                    DateOfBirth = new DateTime(1990, 5, 2),
                    SpecialNumber = 23
                },
                ComparisonB = new SomeTypeParameters()
                {
                    Forename = "David",
                    DateOfBirth = new DateTime(1990, 5, 2),
                    SpecialNumber = 23
                },
                ComparisonC = new SomeTypeParameters()
                {
                    Forename = "David",
                    DateOfBirth = new DateTime(1990, 5, 2),
                    SpecialNumber = 23
                },
                ComparisonD = new SomeTypeParameters()
                {
                    Forename = "Janet",
                    DateOfBirth = new DateTime(2000, 11, 22),
                    SpecialNumber = 23
                },
                ComparisonResponseForAB = new ComparisonResponse(),
                ComparisonResponseForCD = new ComparisonResponse()
            };


            return HomeModel;
        }
    }
}
