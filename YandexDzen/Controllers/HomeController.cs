using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YandexDzen.Models;
using System.Diagnostics;


namespace YandexDzen.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContex db;
        private IWebHostEnvironment _app;
        public HomeController(ApplicationContex context, IWebHostEnvironment app)
        {
            db = context;
            _app = app;
        }


        [HttpPost]
        public async Task<IActionResult> Avtoriz(User user)
        {
            var adm = "Admin";

            if (user.Login == "adm" && user.Password == "adm")
            {
                HttpContext.Session.SetString("LoginUser", adm);
                return RedirectToAction("index");
            }
            var userLogin = await db.Users.FirstOrDefaultAsync(predicate => predicate.Login == user.Login && predicate.Password == user.Password); //проверка пароля
            if (userLogin != null)
            {
                HttpContext.Session.SetString("LoginUser", userLogin.Login);
                return RedirectToAction("HomeMain");
            }
            //else if(userLogin == await db.Users.FirstOrDefaultAsync(predicate => predicate.Login == "admin" && predicate.Password == "123"))
            //{
            //    return RedirectToAction("index");
            //}
            return NotFound();
        }




        public async Task<IActionResult> Postone(int? id)
        {
            if (id != null)
            {
                PostViewModel post = await db.postControlers.FirstOrDefaultAsync(predicate => predicate.id_Post == id);
                if (post != null)
                   // return View(post);
                return RedirectToAction("HomeMain");
            }
            return NotFound();
        }



        public IActionResult Registr()
        {
            return View();
        }

       
        public async Task<IActionResult> HomeMain()
        {
            IQueryable<PostViewModel> postViewModels = db.postControlers;

            ViewBag.LoginUser = HttpContext.Session.GetString("LoginUser");

            return View(await postViewModels.AsNoTracking().ToListAsync());
        }

        

        public IActionResult CreatePost()
        {
           
            ViewBag.LoginUser = HttpContext.Session.GetString("LoginUser");
            return View();
            
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(User user, IFormFile CRFile, PostViewModel post)
        {
           

            var userLogin = await db.Users.FirstOrDefaultAsync(predicate => predicate.Login == user.Login);
            
            string path1 = "";

            if (user.Login == null)
            {

                if (CRFile != null && user != null)
                {
                    path1 = "/File/" + CRFile.FileName;
                    using (var fileStream = new FileStream(_app.WebRootPath + path1, FileMode.Create))
                    {
                        await CRFile.CopyToAsync(fileStream);
                    }
                    FileModel fileModel = new FileModel
                    {
                        Name = CRFile.FileName,
                        Path = path1
                    };

                    //file.Name = formFile.FileName;
                    //file.Path = path1;




                    db.FileModels.Add(fileModel);
                    await db.SaveChangesAsync();
                    // return RedirectToAction("Index");
                }
                //PostViewModel postViewModel = new PostViewModel
                //{
                //    path = path1,
                //    //LoginUser = userLogin
                //};

                post.path = path1;
                post.LoginUser = HttpContext.Session.GetString("LoginUser");
                db.postControlers.Add(post);
                await db.SaveChangesAsync();
                return RedirectToAction("HomeMain");
            }
            return NotFound();
        }

        [HttpGet]
        [ActionName("DeletePost")]
        public async Task<IActionResult> ConfirmDelete1(int? id, User user,PostViewModel post)
        {

                if (id != null)
                {

                    PostViewModel postViewModel = await db.postControlers.FirstOrDefaultAsync(predicate => predicate.id_Post == id);
                    if (postViewModel != null)
                        return View(postViewModel);
                }
            
           return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int? id, User user, PostViewModel post)
        {

            var userLogin = await db.Users.FirstOrDefaultAsync(predicate => predicate.Login == post.LoginUser);
            


            
            

                if (id != null)
                {
                    PostViewModel postViewModel = await db.postControlers.FirstOrDefaultAsync(predicate => predicate.id_Post == id);
                    if (postViewModel != null)
                    {
                        db.postControlers.Remove(postViewModel);
                        await db.SaveChangesAsync();
                        return RedirectToAction("HomeMain");
                    }
                }
            
            
            return NotFound();
        }


        [HttpPost]
       public async Task<IActionResult> Registr(User user)
        {
            try
            {
                var userLogin = await db.Users.FirstOrDefaultAsync(predicate => predicate.Login == user.Login || predicate.Email == user.Email);

                if (userLogin == null)
                {
                    db.Users.Add(user);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Avtoriz");

                }
            }
            catch
            {
                return NotFound();
            }
            return NotFound();
            // db.Users.Add(user);
            // await db.SaveChangesAsync();
            // return RedirectToAction("Avtoriz");
        }

        public IActionResult Avtoriz()
        {
            return View();
        }

       

        public IActionResult AddFile()
        {
            return View(db.FileModels.ToList());
        }

        [HttpPost]  
        public async Task<IActionResult> AddFile(IFormFile formFile)
        {
            if (formFile != null)
            {
                string path = "/File/" + formFile.FileName;
                using (var fileStream = new FileStream(_app.WebRootPath + path, FileMode.Create))
                {
                    await formFile.CopyToAsync(fileStream);
                }
                FileModel fileModel = new FileModel
                {
                    Name = formFile.FileName,
                    Path = path
                };
                db.FileModels.Add(fileModel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("AddFile");
        }



        public async Task<IActionResult> Index(int? id, string login, int page = 1, StateSort sortOrder = StateSort.IdAsc)
        {
            ViewBag.LoginUser = HttpContext.Session.GetString("LoginUser");
            IQueryable<User> users = db.Users;
            //Фильтрация или поиск
            if (id != null && id > 0)
            {
                users = users.Where(p => p.Id_user == id);
            }
            if (!String.IsNullOrEmpty(login))
            {
                users = users.Where(p => p.Email.Contains(login));
            }
            //Сортировка
            switch (sortOrder)
            {
                case StateSort.IdAsc:
                    {
                        users = users.OrderBy(m => m.Id_user);
                        break;
                    }
                case StateSort.IdDesc:
                    {
                        users = users.OrderByDescending(m => m.Id_user);
                        break;
                    }
                case StateSort.EmailAsc:
                    {
                        users = users.OrderBy(m => m.Email);
                        break;
                    }
                case StateSort.EmailDesc:
                    {
                        users = users.OrderByDescending(m => m.Email);
                        break;
                    }
            }
            //Пагинация
            int pageSize = 8;
            var count = await users.CountAsync();
            var item = await users.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            IndexViewModel indexViewModel = new IndexViewModel
            {
                FilterViewModel = new FilterViewModel(id, login),
                SortViewModel = new SortViewModel(sortOrder),
                PageViewModel = new PageViewModel(count, page, pageSize),
                Users = item

            };

            return View(indexViewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(predicate => predicate.Id_user == id);
                if (user != null)
                    return View(user);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(predicate => predicate.Id_user == id);
                if (user != null)
                {
                    db.Users.Remove(user);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(predicate => predicate.Id_user == id);
                if (user != null)
                    return View(user);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(User user)
        {
            db.Users.Update(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(predicate => predicate.Id_user == id);
                if (user != null)
                    return View(user);
            }
            return NotFound();
        }
    }
}