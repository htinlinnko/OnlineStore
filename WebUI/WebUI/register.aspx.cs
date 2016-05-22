using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebUI
{
    public partial class register : System.Web.UI.Page
    {
        main_infoService.Imain_infoClient mainInfoClient = new main_infoService.Imain_infoClient();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                mainInfoClient.setMainUserInformation(txtFirstName.Text, txtLastName.Text, 
                    txtEmail.Text, chkPDPA.Checked ? 1 : 0);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void clearForm()
        {
            try
            {

            }
            catch (Exception ex) { throw ex; }
        }
    }
}