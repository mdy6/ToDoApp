using Microsoft.AspNetCore.Mvc;
using ToDoApp.Components.Extensions;
using ToDoApp.Components.Security;
using ToDoApp.Objects;
using ToDoApp.Resources;
using ToDoApp.Services;
using ToDoApp.Validators;

namespace ToDoApp.Controllers
{
    [AllowUnauthorized]
    public class Profile : ValidatedController<IAccountValidator, IAccountService>
    {
        public Profile(IAccountValidator validator, IAccountService service)
            : base(validator, service)
        {
        }

        [HttpGet]
        public ActionResult Edit()
        {
            if (!Service.IsActive(User.Id()))
                return RedirectToAction(nameof(Auth.Logout), nameof(Auth));

            return View(Service.Get<ProfileEditView>(User.Id()));
        }

        [HttpPost]
        public ActionResult Edit(ProfileEditView profile)
        {
            profile.Id = User.Id();

            if (!Service.IsActive(profile.Id))
                return RedirectToAction(nameof(Auth.Logout), nameof(Auth));

            if (!Validator.CanEdit(profile))
                return View(profile);

            Service.Edit(User, profile);

            Alerts.AddSuccess(Message.For<AccountView>("ProfileUpdated"), 4000);

            return RedirectToAction(nameof(Edit));
        }

        [HttpGet]
        public ActionResult Delete()
        {
            if (!Service.IsActive(User.Id()))
                return RedirectToAction(nameof(Auth.Logout), nameof(Auth));

            Alerts.AddWarning(Message.For<AccountView>("ProfileDeleteDisclaimer"));

            return View();
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(ProfileDeleteView profile)
        {
            profile.Id = User.Id();

            if (!Service.IsActive(profile.Id))
                return RedirectToAction(nameof(Auth.Logout), nameof(Auth));

            if (!Validator.CanDelete(profile))
            {
                Alerts.AddWarning(Message.For<AccountView>("ProfileDeleteDisclaimer"));

                return View();
            }

            Service.Delete(profile.Id);

            Authorization.Refresh(HttpContext.RequestServices);

            return RedirectToAction(nameof(Auth.Logout), nameof(Auth));
        }
    }
}
