using GymMangmentSystem.BLL.Services.Interfaces;
using GymMangmentSystem.BLL.ViewModels.MemberViewModels;
using GymMangmentSystem.DAL.Data.Models;
using GymMangmentSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymMangmentSystem.PL.Controllers
{
    public class MembersController : Controller
    {
        public IMemberService _memberService ; 
        public MembersController( IMemberService memberService)
        {
            _memberService = memberService;
        }

        //done
        #region Display All Members
        //list of all members , baseurl/Members/Index
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memberService.GetAllMembersAsync(ct: ct);
            return View(members);
        }

        #endregion 

        //done
        #region Display Member Details
        //Member details by id
        // GET: baseurl/Members/Details/5
        public async Task<IActionResult> MemberDetails(int id , CancellationToken ct)
        {
            var member = await _memberService.GetMemberDetailsAsync(id, ct);
            if(member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        #endregion

        //done
        #region Display Member's Health Record
        //Health record of member by id
        // GET: baseurl/Members/HealthRecord/5
        public async Task<IActionResult> HealthRecordDetails (int id , CancellationToken ct)
        {
            var healthRecord = await _memberService.GetMemberHealthRecordAsync(id, ct);
            if (healthRecord is null)
            {
                TempData["ErrorMessage"] = "Health Record Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(healthRecord);
        }

        #endregion

        //done
        #region Create Member

        #region Display Create Form
        //Creat - Display Form
        // GET: baseurl/Members/Creat 
        public IActionResult Create()
        {
            return View();
        }

        #endregion

        #region Submit Create Form
        //Creat - Submit Form
        // Post: baseurl/Members/Creat Member
        [HttpPost]
        public async Task<IActionResult> Create(CreateMemberViewModel model , CancellationToken ct)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _memberService.CreateMemberAsync(model, ct);
            if (result)
            {
                TempData["SuccessMessage"] = "Member Created Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Create Member";
            }
                return RedirectToAction(nameof(Index));
        }

        #endregion

        #endregion    

        //done
        #region Edit Member

        #region Display Edit Form
        //Edit - Display form
        // GET: baseurl/Members/Edit/5
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberToUpdateAsync(id, ct);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        #endregion

        #region Submit Edit Form
        //Edit - Submit form
        // Post: baseurl/Members/Edit/5 Member
        [HttpPost]
        public async Task<IActionResult> EditMember ([FromRoute]int id , MemberToUpdateViewModel model , CancellationToken ct)
        {
            if (!ModelState.IsValid) { 
                return View(model);
            }

            var result = await _memberService.UpdateMemberDetailsAsync(id, model, ct);
            if(result == false)
            {
                TempData["ErrorMessage"] = "Failed To Update Member";
                return View(model);
            }

            TempData["SuccessMessage"] = "Member Updated Successfully";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #endregion


        #region Delete Member

        #region Display Delete Form
        //Delete - Display Delete form
        // GET: baseurl/Members/Delete/5

        public async Task<IActionResult> Delete(int id , CancellationToken ct)
        {
            var result = await _memberService.GetMemberDetailsAsync(id,ct);
            if (result is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(result);
        }

        #endregion

        #region Submit Delete Form
        //Delete - Submit Delete form : Delete confirmation
        // Post: baseurl/Members/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id , CancellationToken ct)
        {
            var result = await _memberService.RemoveMemberAsync(id, ct);
            if (result)
            {
                TempData["SuccessMessage"] = "Member Deleted Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to Delete Member";
            }

            return RedirectToAction(nameof(Index));
        }


        #endregion


        #endregion

    }
}
