using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //For to show Product
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();
            return View(objProductList);
        }
        //For to show create view
        public IActionResult Create()
        {
            //To get categories by using Projection in EF core
            //IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().
            //    Select(u => new SelectListItem
            //    {
            //        Text = u.Name,
            //        Value = u.Id.ToString()

            //    });
            //ViewBag
            //ViewBag.CategoryList = CategoryList;

            //ViewData
            //ViewData["CategoryList"] = CategoryList;

            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };

            return View(productVM);
        }

        //For to create Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductVM obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(obj.Product);
                _unitOfWork.Save();
                TempData["Success"] = "Product added successfully";
                return RedirectToAction("Index");
            }
            else 
            {
                obj.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });

                return View(obj); // return to the form with validation errors
            }
           
        }

        //For to show Edit view
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            //Three way to do it.
            Product? ProductfromDb = _unitOfWork.Product.Get(u => u.Id == id);
            //Product? ProductfromDb1 = _db.Categories.Find(id);
            //Product? ProductfromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();

            if (ProductfromDb == null)
            {
                return NotFound();
            }
            return View(ProductfromDb);
        }

        //For to Edit Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                TempData["SuccessUp"] = "Product updated successfully";
                return RedirectToAction("Index");
            }
            return View(obj); // return to the form with validation errors
        }
        //For to show Delete view
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? ProductfromDb = _unitOfWork.Product.Get(u => u.Id == id);
            if (ProductfromDb == null)
            {
                return NotFound();
            }
            return View(ProductfromDb);
        }

        //For to Delete Product
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            Product? obj = _unitOfWork.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            TempData["SuccessDel"] = "Product deleted successfully";
            return RedirectToAction("Index");

        }
    }
}
