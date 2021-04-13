using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TiViC_proba.Models;
using Cassandra;

namespace TiViC_proba.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private string userId;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            userId = "1";
        }

        public IActionResult Index(Programi_po_KanaluViewModel? model)
        {

            ISession session = SessionManager.GetSession();
            List<Kanal> kanali = new List<Kanal>();

            if (session == null)
                return View();

            var data = session.Execute("SELECT * FROM \"Kanal\"");

            foreach (var ch in data)
            {
                Kanal kanal = new Kanal();
                kanal.kanalId = ch["kanalId"] != null ? ch["kanalId"].ToString() : string.Empty;
                kanal.naziv = ch["naziv"] != null ? ch["naziv"].ToString() : string.Empty;
                kanali.Add(kanal);
            }

            model.Kanali = kanali;

            Programi_po_KanaluViewModel viewModel = new Programi_po_KanaluViewModel()
            {
                Kanali = kanali
            };

            List<string> datumi = new List<string>();
            DateTime startOfWeek = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek) + 1);
            datumi.Add(startOfWeek.Date.ToShortDateString());
            while (datumi.Count < 7)
            {
                startOfWeek = startOfWeek.AddDays(1);
                datumi.Add(startOfWeek.Date.ToShortDateString());
            }
            model.Datumi = datumi;

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult Details(Programme prog)
        {
            ISession session = SessionManager.GetSession();
            if (session == null)
                return RedirectToPage("Index");

            var programme = session.Execute("SELECT * FROM \"Programi3\" WHERE \"kanalId\" = '" + prog.kanalId + "' AND datum = '2021-01-25' AND vreme = '" + prog.vreme + "'");

            foreach (var p in programme)
            {
                prog.naziv = p["naziv"] != null ? p["naziv"].ToString() : string.Empty;
                prog.opis = p["opis"] != null ? p["opis"].ToString() : string.Empty;
                prog.tip = p["tip"] != null ? p["tip"].ToString() : string.Empty;
            }
            return View("Details", prog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchByChannel(Programi_po_KanaluViewModel model)
        {
            ISession session = SessionManager.GetSession();
            List<Programme> programi = new List<Programme>();

            if (session == null)
                return View();

            //var data = session.Execute("SELECT * FROM \"Programi_po_Kanalu\" WHERE \"kanalId\" = '" + model.KanalId + "' AND datum = '" +
            //                            model.Datum + "'");
            var data = session.Execute("SELECT * FROM \"Programi_po_Kanalu\" WHERE \"kanalId\" = '" + model.KanalId + "' AND datum = '" + model.Datum + "' ORDER BY vreme");

            foreach (var prog in data)
            {
                Programme program = new Programme();
                program.kanalId = prog["kanalId"] != null ? prog["kanalId"].ToString() : string.Empty;
                program.datum = prog["datum"] != null ? prog["datum"].ToString() : string.Empty;
                program.vreme = prog["vreme"] != null ? prog["vreme"].ToString() : string.Empty;
                program.naziv = prog["naziv"] != null ? prog["naziv"].ToString() : string.Empty;
                program.opis = prog["opis"] != null ? prog["opis"].ToString() : string.Empty;
                program.tip = prog["tip"] != null ? prog["tip"].ToString() : string.Empty;
                programi.Add(program);
            }


            model.Programi = programi;


            List<Kanal> kanali = new List<Kanal>();

            if (session == null)
                return View();

            var kanalData = session.Execute("SELECT * FROM \"Kanal\"");

            foreach (var ch in kanalData)
            {
                Kanal kanal = new Kanal();
                kanal.kanalId = ch["kanalId"] != null ? ch["kanalId"].ToString() : string.Empty;
                kanal.naziv = ch["naziv"] != null ? ch["naziv"].ToString() : string.Empty;
                kanali.Add(kanal);
            }

            model.Kanali = kanali;

            List<string> datumi = new List<string>();
            DateTime startOfWeek = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek) + 1);
            datumi.Add(startOfWeek.Date.ToShortDateString());
            while (datumi.Count < 7)
            {
                startOfWeek = startOfWeek.AddDays(1);
                datumi.Add(startOfWeek.Date.ToShortDateString());
            }
            model.Datumi = datumi;

            return View("Index", model);
        }

        public ActionResult AddToWatchLater(Programme programModel)
        {
            ISession session = SessionManager.GetSession();

            if (session == null)
                return View();
            //promeni userId
            session.Execute("INSERT INTO \"Watch_later2\" (\"userId\", \"kanalId\", datum, vreme, naziv, opis, tip) VALUES ('" + this.userId + "', '" + programModel.kanalId + "', '" + programModel.datum + "', '" + programModel.vreme + "', '" + programModel.naziv + "', '" + programModel.opis + "', '" + programModel.tip + "')");

            return View("Details", programModel);
        }

        public ActionResult WatchLater()
        {
            ISession session = SessionManager.GetSession();

            if (session == null)
                return View();

            List<WatchLater> programi = new List<WatchLater>();

            if (userId != null)
            {
                var data = session.Execute("SELECT * FROM \"Watch_later2\" WHERE \"userId\" = '" + userId + "'");

                foreach (var prog in data)
                {
                    WatchLater program = new WatchLater();
                    program.userId = prog["userId"] != null ? prog["userId"].ToString() : string.Empty;
                    program.kanalId = prog["kanalId"] != null ? prog["kanalId"].ToString() : string.Empty;
                    program.datum = prog["datum"] != null ? prog["datum"].ToString() : string.Empty;
                    program.vreme = prog["vreme"] != null ? prog["vreme"].ToString() : string.Empty;
                    program.naziv = prog["naziv"] != null ? prog["naziv"].ToString() : string.Empty;
                    program.opis = prog["opis"] != null ? prog["opis"].ToString() : string.Empty;
                    program.tip = prog["tip"] != null ? prog["tip"].ToString() : string.Empty;
                    programi.Add(program);
                }
            }

            return View(programi);
        }

        
        public ActionResult ObrisiIzWatchLater(WatchLater program)
        {
            ISession session = SessionManager.GetSession();

            if (session == null)
                return View();

            session.Execute("DELETE FROM \"Watch_later2\" WHERE \"userId\" = '" + program.userId + "' AND \"kanalId\" = '" + program.kanalId + "' AND datum = '" + program.datum + "' AND vreme = '" + program.vreme + "'");

            List<WatchLater> programi = new List<WatchLater>();

            var data = session.Execute("SELECT * FROM \"Watch_later2\" WHERE \"userId\" = '" + userId + "'");

            foreach (var prog in data)
            {
                WatchLater p = new WatchLater();
                p.userId = prog["userId"] != null ? prog["userId"].ToString() : string.Empty;
                p.kanalId = prog["kanalId"] != null ? prog["kanalId"].ToString() : string.Empty;
                p.datum = prog["datum"] != null ? prog["datum"].ToString() : string.Empty;
                p.vreme = prog["vreme"] != null ? prog["vreme"].ToString() : string.Empty;
                p.naziv = prog["naziv"] != null ? prog["naziv"].ToString() : string.Empty;
                p.opis = prog["opis"] != null ? prog["opis"].ToString() : string.Empty;
                p.tip = prog["tip"] != null ? prog["tip"].ToString() : string.Empty;
                programi.Add(p);
            }

            return View("WatchLater", programi);
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult DodajUsera(User model)
        {
            ISession session = SessionManager.GetSession();

            var data = session.Execute("SELECT * FROM \"User\"");
            var maxId = 0;
            if (data != null)
            {
                int temp = 0;
                foreach (var user in data)
                {
                    temp = Int32.Parse(user["userId"].ToString());
                    if (maxId < temp)
                        maxId = temp;
                }
            }
            else
                maxId = 1;
            session.Execute("INSERT INTO \"User\" (\"userId\", ime) VALUES ('" + ++maxId + "', '" + model.ime + "')");

            return View("Register");
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult LoginUser(User model)
        {
            this.userId = model.userId;
            
            ISession session = SessionManager.GetSession();
            List<Kanal> kanali = new List<Kanal>();

            if (session == null)
                return View();

            Programi_po_KanaluViewModel viewModel = new Programi_po_KanaluViewModel();
            var data = session.Execute("SELECT * FROM \"Kanal\"");

            foreach (var ch in data)
            {
                Kanal kanal = new Kanal();
                kanal.kanalId = ch["kanalId"] != null ? ch["kanalId"].ToString() : string.Empty;
                kanal.naziv = ch["naziv"] != null ? ch["naziv"].ToString() : string.Empty;
                kanali.Add(kanal);
            }

            viewModel.Kanali = kanali;
            
            List<string> datumi = new List<string>();
            DateTime startOfWeek = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek) + 1);
            datumi.Add(startOfWeek.Date.ToShortDateString());
            while (datumi.Count < 7)
            {
                startOfWeek = startOfWeek.AddDays(1);
                datumi.Add(startOfWeek.Date.ToShortDateString());
            }
            viewModel.Datumi = datumi;

            return View("Index", viewModel);
        }

    }
}
