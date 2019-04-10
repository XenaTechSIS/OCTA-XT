using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using FSP.Domain.Model;
using FSP.Web.Areas.AdminArea.ViewModels;
using FSP.Web.Filters;

namespace FSP.Web.Areas.AdminArea.Controllers
{
    [CustomAuthorization(Roles = "Admin")]
    public class ContractController : Controller
    {
        private readonly FSPDataContext _db = new FSPDataContext();

        public ActionResult Create()
        {
            var model = new ContractCreateViewModel();
            var contract = new Contract
            {
                EndDate = DateTime.Today,
                StartDate = DateTime.Today
            };
            model.Contract = contract;
            model.SelectedBeats = new List<Guid>();

            ViewBag.Contractors = _db.Contractors.OrderBy(p => p.ContractCompanyName).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ContractCreateViewModel contractCreateViewModel)
        {
            if (ModelState.IsValid)
            {
                //if Contract does not exist yet       
                var contractId = Guid.NewGuid();
                contractCreateViewModel.Contract.ContractID = contractId;
                contractCreateViewModel.Contract.BeatID = _db.vBeats.FirstOrDefault().BeatID; //obsolete
                _db.Contracts.InsertOnSubmit(contractCreateViewModel.Contract);
                _db.SubmitChanges();

                //add beats
                foreach (var beat in contractCreateViewModel.SelectedBeats)
                {
                    var contractsBeat = new ContractsBeat
                    {
                        BeatID = beat,
                        ContractID = contractId
                    };
                    _db.ContractsBeats.InsertOnSubmit(contractsBeat);
                }

                _db.SubmitChanges();

                return RedirectToAction("Index");
            }

            ViewBag.Contractors = _db.Contractors.OrderBy(p => p.ContractCompanyName).ToList();

            return View(contractCreateViewModel);
        }

        public ActionResult Delete(Guid id)
        {
            var contract = _db.Contracts.Single(r => r.ContractID == id);
            if (contract == null) return HttpNotFound();
            return View(contract);
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var contract = _db.Contracts.Single(r => r.ContractID == id);
            _db.Contracts.DeleteOnSubmit(contract);
            _db.SubmitChanges();

            var existingBeats = _db.ContractsBeats.Where(p => p.ContractID == id);

            foreach (var existingBeat in existingBeats) _db.ContractsBeats.DeleteOnSubmit(existingBeat);

            _db.SubmitChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Edit(Guid id)
        {
            var contract = _db.Contracts.Single(r => r.ContractID == id);

            if (contract == null) return HttpNotFound();

            var model = new ContractCreateViewModel
            {
                Contract = contract,
                SelectedBeats = new List<Guid>()
            };

            ViewBag.Contractors = _db.Contractors.OrderBy(p => p.ContractCompanyName).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ContractCreateViewModel contractCreateViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Contracts.Attach(contractCreateViewModel.Contract);
                    _db.Refresh(RefreshMode.KeepCurrentValues, contractCreateViewModel.Contract);
                    _db.SubmitChanges();

                    //remove existing ones
                    var existingBeats =
                        _db.ContractsBeats.Where(p => p.ContractID == contractCreateViewModel.Contract.ContractID);

                    foreach (var existingBeat in existingBeats) _db.ContractsBeats.DeleteOnSubmit(existingBeat);

                    _db.SubmitChanges();

                    //re-add beats
                    foreach (var beat in contractCreateViewModel.SelectedBeats)
                    {
                        var contractsBeat = new ContractsBeat();
                        contractsBeat.BeatID = beat;
                        contractsBeat.ContractID = contractCreateViewModel.Contract.ContractID;
                        _db.ContractsBeats.InsertOnSubmit(contractsBeat);
                    }

                    _db.SubmitChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            ViewBag.Contractors = _db.Contractors.OrderBy(p => p.ContractCompanyName).ToList();
            return View(contractCreateViewModel);
        }

        public ActionResult Index()
        {
            var dbContracts = _db.Contracts.OrderBy(p => p.Contractor.ContractCompanyName).ToList();
            var dbBeats = _db.Beats_News.Where(p => p.Active).OrderBy(p => p.BeatNumber).ToList();
            var dbContractsBeats = _db.ContractsBeats.ToList();

            var model = new ContractIndexViewModel();
            model.Contracts = new List<ContractIndex>();

            foreach (var dbContract in dbContracts)
            {
                var selectedBeats = (from q in dbContractsBeats
                                     join b in dbBeats on q.BeatID equals b.BeatID
                                     where q.ContractID == dbContract.ContractID
                                     orderby b.BeatNumber
                                     select b.BeatNumber).ToList();

                model.Contracts.Add(new ContractIndex
                {
                    Contract = dbContract,
                    SelectedBeats = selectedBeats.OrderBy(p => p).ToList()
                });
            }

            model.Contracts = model.Contracts.OrderBy(p => p.Contract.Contractor.ContractCompanyName)
                .ThenBy(p => p.SelectedBeats.FirstOrDefault()).ToList();

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}