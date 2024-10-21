using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _23DH114638_MyStore.Models;
using _23DH114638_MyStore.Models.ViewModel;
using PagedList;

namespace _23DH114638_WebMayTinh.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Products
        public ActionResult Index(string SearchTerm, decimal? minprice,decimal? maxprice,string sortorder,int? page)
        {
            var model = new ProductSearchVM();
            var products = db.Products.AsQueryable(); //Lấy tất cả danh sách của DB mà có thể query nó 
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                //Tìm kiếm dựa trên từ khóa
                model.SearchTerm = SearchTerm;
                products = products.Where(p => p.ProductName.Contains(SearchTerm) ||
                p.ProductDescription.Contains(SearchTerm) ||
                p.Category.CategoryName.Contains(SearchTerm));
            }

            //So sánh giá trị các trường product price trong database với giá trị min ta nhập vào
            if (minprice.HasValue)
            {
                model.MinPrice=minprice.Value;
                products = products.Where(p => p.ProductPrice >= minprice.Value);
            }



            //So sánh giá trị các trường product price trong database với giá trị max ta nhập vào
            if (minprice.HasValue)
            {
                model.MaxPrice=maxprice.Value;
                products = products.Where(p => p.ProductPrice <= maxprice.Value);
            }


            //SortOrder
            switch (sortorder)
            {
                case "name_asc":
                    products=products.OrderBy(p => p.ProductName);
                    break;
                case "name_desc":
                    products=products.OrderByDescending(p => p.ProductName);
                    break;
                case "price_asc":
                    products = products.OrderBy(p => p.ProductPrice);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.ProductPrice);
                    break;
                default: //Mặc định sắp xếp theo tên
                    products = products = products.OrderBy(p => p.ProductName);
                    break;
            }
            model.SortOrder = sortorder;
            //Đoạn code liên quan tới phân trang
            int pageNumber = page ?? 1; //lấy số trang hiện tại ( mặc định là trang 1 nếu ko có giá trị)
            int pagesize = 3; //Số sản phẩm có thể hiển thị được ở 1 trang

            //Đóng câu lệnh này,sửn dụng ToPageList để lấy danh sách đã phân trang
            //model.Products = products.ToList(); 

            model.Products=products.ToPagedList(pageNumber, pagesize);
            return View(model);
        }

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,ProductPrice,ProductImage,ProductDescription")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductPrice,ProductImage,ProductDescription")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
