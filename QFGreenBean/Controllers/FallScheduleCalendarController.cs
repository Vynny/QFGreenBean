using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DayPilot.Web.Mvc;
using DayPilot.Web.Mvc.Enums;
using DayPilot.Web.Mvc.Events.Calendar;
using QFGreenBean.Models;

namespace QFGreenBean.Controllers
{
    public class FallScheduleCalendarController : Controller
    {
        // GET: FallScheduleCalendar
        public ActionResult Backend()
        {
            return new Dpc().CallBack(this);
        }

        public class Dpc : DayPilotCalendar
        {
            private PlannerDbEntities db = new PlannerDbEntities();

            protected override void OnInit(InitArgs e)
            {
                Update();
            }

            protected override void OnFinish()
            {
                if (UpdateType == CallBackUpdateType.None)
                {
                    return;
                }

                DataIdField = "FallEventId";
                DataStartField = "StartTime";
                DataEndField = "EndTime";
                DataTextField = "Description";

                Events = from e in db.FallEvents select e;
            }

        }
    }
}