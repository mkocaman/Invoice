using ESF_kz.SessionService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ESF_kz.Forms
{
	public partial class UserInfoForm : Form
	{
		public UserInfoForm()
		{
			InitializeComponent();
			FormManagerFacade.setUserInfoForm(this);
		}

		internal bool fillUserInfoForm(User user)
		{
			try
			{
				fillUserInfoTab(user);
				if (user.taxpayer != null)
				{
					fillTaxpayerInfoTab(user.taxpayer);
					if (user.taxpayer.accounts.Length > 0)
					{
						fillSettlementAccountsTab(user.taxpayer.accounts);
					}
				}			
				if (user.taxpayer.headOffice != null)
				{
					fillHeadOfficeInfoTab(user.taxpayer.headOffice);
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private void fillSettlementAccountsTab(SettlementAccount[] accounts)
		{
			for (int i = 0; i < accounts.Length; i++)
			{
				tvSettlementAccounts.Nodes[0].Nodes.Add(new TreeNode("Settlement account #"+(i+1)));				
			}
			fillSettlementAccountTab(accounts[0]);
			tvSettlementAccounts.ExpandAll();
		}

		private void fillSettlementAccountTab(SettlementAccount settlementAccount)
		{
			tbSettlementAccountId.Text = settlementAccount.id.ToString();
			tbSettlementAccountTaxpayerTin.Text = settlementAccount.taxpayerTin;
			tbSettlementAccountBankId.Text = settlementAccount.bank.id.ToString();
			tbSettlementAccountBankNameRu.Text = settlementAccount.bank.nameRu;
			tbSettlementAccountBankNameKz.Text = settlementAccount.bank.nameKz;
			tbSettlementAccountBankBik.Text = settlementAccount.bank.bik;
			tbSettlementAccountBankCode.Text = settlementAccount.bank.code;
			tbSettlementAccountBankTin.Text = settlementAccount.bank.tin;
			tbSettlementAccountBankRnn.Text = settlementAccount.bank.rnn;
			chbxSettlementAccountBankActive.Checked = settlementAccount.bank.active;
			tbSettlementAccountType.Text = settlementAccount.accountType.ToString();
			tbSettlementAccount.Text = settlementAccount.account;
			tbSettlementAccountDateOpen.Text = settlementAccount.dateOpen.ToShortDateString();
			tbSettlementAccountDateClose.Text = settlementAccount.dateOpen.ToShortDateString();
		}

		private void fillHeadOfficeInfoTab(Taxpayer headOffice)
		{
			tbHeadOfficeTin.Text = headOffice.tin;
			tbHeadOfficeNameRu.Text = headOffice.nameRu;
			tbHeadOfficeLastNameRu.Text = headOffice.lastNameRu;
			tbHeadOfficeFirstNameRu.Text = headOffice.firstNameRu;
			tbHeadOfficeMiddleNameRu.Text = headOffice.middleNameRu;
			tbHeadOfficeLastNameKz.Text = headOffice.lastNameKz;
			tbHeadOfficeFirstNameKz.Text = headOffice.firstNameKz;
			tbHeadOfficeMiddleNameKz.Text = headOffice.middleNameKz;
			tbHeadOfficeAddressRu.Text = headOffice.addressRu;
			tbHeadOfficeAddressKz.Text = headOffice.addressKz;
			tbHeadOfficeCertificateCeries.Text = headOffice.certificateSeries;
			tbHeadOfficeCertificateNum.Text = headOffice.certificateNum;
			chbxHeadOfficeIsResident.Checked = headOffice.resident;
			switch (headOffice.type)
			{
				case EnterpriseType.NOT_SET:
					tbHeadOfficeType.Text = "Отсутствует в справочнике";
					break;
				case EnterpriseType.INDIVIDUAL:
					tbHeadOfficeType.Text = "Физическое лицо";
					break;
				case EnterpriseType.INDIVIDUAL_ENTREPRENEUR:
					tbHeadOfficeType.Text = "Индивидуальный предприниматель";
					break;
				case EnterpriseType.PRIVATE_ENTERPRISE:
					tbHeadOfficeType.Text = "Частное предприятие";
					break;
				case EnterpriseType.STATE_ENTERPRISE:
					tbHeadOfficeType.Text = "Частное предприятие";
					break;
				default:
					tbHeadOfficeType.Text = headOffice.type.ToString();
					break;
			}			
			tbHeadOfficeEnterpriseAdministrator.Text = headOffice.enterpriseAdministrator;
			tbHeadOfficeBudgetIin.Text = headOffice.budgetIin;
			tbHeadOfficeKogd.Text = headOffice.kogd;
		}

		internal bool fillUserInfoForm(profileInfo[] profileInfos)
		{
			try
			{
				if (profileInfos.Length >0)
				{
					for (int i = 0; i < profileInfos.Length; i++)
					{
						tvProfileInfo.Nodes[0].Nodes.Add(new TreeNode("Profile #" + (i + 1)));
						//tvProfileInfo.Nodes[0].Nodes[i].c
					}
					fillProfileInfo(profileInfos[0]);
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private void fillTaxpayerInfoTab(Taxpayer taxpayer)
		{
			tbTaxpayerTin.Text = taxpayer.tin;
			tbTaxpayerNameRu.Text = taxpayer.nameRu;
			tbTaxpayerLastNameRu.Text = taxpayer.lastNameRu;
			tbTaxpayerFirstNameRu.Text = taxpayer.firstNameRu;
			tbTaxpayerMiddleNameRu.Text = taxpayer.middleNameRu;
			tbTaxpayerLastNameKz.Text = taxpayer.lastNameKz;
			tbTaxpayerFirstNameKz.Text = taxpayer.firstNameKz;
			tbTaxpayerMiddleNameKz.Text = taxpayer.middleNameKz;
			tbTaxpayerAddressRu.Text = taxpayer.addressRu;
			tbTaxpayerAddressKz.Text = taxpayer.addressKz;
			tbTaxpayerCerificateCeries.Text = taxpayer.certificateSeries;
			tbTaxpayerCertificateNum.Text = taxpayer.certificateNum;
			chbxTaxpayerIsResident.Checked = taxpayer.resident;
			switch (taxpayer.type)
			{
				case EnterpriseType.NOT_SET:
					tbTaxpayerType.Text = "Отсутствует в справочнике";
					break;
				case EnterpriseType.INDIVIDUAL:
					tbTaxpayerType.Text = "Физическое лицо";
					break;
				case EnterpriseType.INDIVIDUAL_ENTREPRENEUR:
					tbTaxpayerType.Text = "Индивидуальный предприниматель";
					break;
				case EnterpriseType.PRIVATE_ENTERPRISE:
					tbTaxpayerType.Text = "Частное предприятие";
					break;
				case EnterpriseType.STATE_ENTERPRISE:
					tbTaxpayerType.Text = "Государственное предприятие/учреждение";
					break;
				default:
					tbTaxpayerType.Text = taxpayer.type.ToString();
					break;
			}
			tbTaxpayerEnterpriseAdministrator.Text = taxpayer.enterpriseAdministrator;
			tbTaxpayerBudgetIin.Text = taxpayer.budgetIin;
			tbTaxpayerKogd.Text = taxpayer.kogd;
		}

		private void fillUserInfoTab(User user)
		{
			tbUserLogin.Text = user.login;
			tbUserEmail.Text = user.email;
			tbUserMobile.Text = user.mobile;
			tbUserIssueDate.Text = user.issueDate;
			tbUserIssuedBy.Text = user.issuedBy;
			tbUserPassportNum.Text = user.passportNum;

			switch (user.status)
			{
				case userStatusType.ACTIVE:
					tbUserStatus.Text = "Активный";
					break;
				case userStatusType.BLOCKED:
					tbUserStatus.Text = "Заблокированный администратором системы";
					break;
				case userStatusType.ENTERPRISE_ADMIN_CHANGE_BLOCKED:
					tbUserStatus.Text = "Заблокированный при смене руководителя предприятия";
					break;
				case userStatusType.ENTERPRISE_BLOCKED:
					tbUserStatus.Text = "Заблокированный при блокировке предприятия";
					break;
				default:
					tbUserStatus.Text = user.status.ToString();
					break;
			}
			
			tbUserReason.Text = user.reason;
		}

		private void l_linkToHeadOfficeTab_Click(object sender, EventArgs e)
		{
			tabControl1.SelectedTab = tabControl1.TabPages[2];
		}

		private void l_linkToAccountsTab_Click(object sender, EventArgs e)
		{
			tabControl1.SelectedTab = tabControl1.TabPages[3];
		}

		private void l_UserTaxpayerInfo_Click(object sender, EventArgs e)
		{
			tabControl1.SelectedTab = tabControl1.TabPages[1];
		}

		private void tvProfileInfo_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if(e.Node.Parent != null)
			{
				fillProfileInfo(SessionDataManagerFacade.getCurrentUserProfilesData()[e.Node.Index]);
			}			
		}

		private void fillProfileInfo(profileInfo profileInfo)
		{
			tbProfileInfoIin.Text = profileInfo.iin;
			tbProfileInfoTin.Text = profileInfo.tin;
			switch (profileInfo.businessProfileType)
			{
				case businessProfileType.ADMIN_ENTERPRISE:
					tbProfileInfoBusinessProfileType.Text = "Руководитель юридического лица";
					break;
				case businessProfileType.USER:
					tbProfileInfoBusinessProfileType.Text = "Пользователь приглашенный в предприятие";
					break;
				case businessProfileType.ENTREPRENEUR:
					tbProfileInfoBusinessProfileType.Text = "Индивидуальный предприниматель";
					break;
				case businessProfileType.ENTREPRENEUR_USER:
					tbProfileInfoBusinessProfileType.Text = "Пользователь работающий в ИП";
					break;
				case businessProfileType.INDIVIDUAL:
					tbProfileInfoBusinessProfileType.Text = "Физическое лицо";
					break;
				case businessProfileType.PROJECT_ADMIN:
					tbProfileInfoBusinessProfileType.Text = "Профиль руководителя проекта (управляет пользователями в рамках проекта)";
					break;
				case businessProfileType.PROJECT_USER:
					tbProfileInfoBusinessProfileType.Text = "Пользователь работающий в рамках проекта";
					break;
				case businessProfileType.LAWYER:
				case businessProfileType.BAILIFF:
				case businessProfileType.MEDIATOR:
				case businessProfileType.NOTARY:
				case businessProfileType.LAWYER_USER:
				case businessProfileType.BAILIFF_USER:
				case businessProfileType.MEDIATOR_USER:
				case businessProfileType.NOTARY_USER:				
				default:
					tbProfileInfoBusinessProfileType.Text = profileInfo.businessProfileType.ToString();
					break;
			}
			switch (profileInfo.status)
			{
				case userStatusType.ACTIVE:
					tbProfileInfoStatus.Text = "Активный";
					break;
				case userStatusType.BLOCKED:
					tbProfileInfoStatus.Text = "Заблокированный администратором системы";
					break;
				case userStatusType.ENTERPRISE_ADMIN_CHANGE_BLOCKED:
					tbProfileInfoStatus.Text = "Заблокированный при смене руководителя предприятия";
					break;
				case userStatusType.ENTERPRISE_BLOCKED:
					tbProfileInfoStatus.Text = "Заблокированный при блокировке предприятия";
					break;
				default:
					tbProfileInfoStatus.Text = profileInfo.status.ToString();
					break;
			}

			switch (profileInfo.type)
			{
				case profileType.GENERAL:
					tbProfileInfoType.Text = "Общий профиль";
					break;
				case profileType.PROJECT:
					tbProfileInfoType.Text = "Профиль созданный в рамках проекта";
					break;
				default:
					tbProfileInfoType.Text = profileInfo.type.ToString();
					break;
			}

			switch (profileInfo.projectParticipantType)
			{
				case projectParticipantType.ATTORNEY:
					tbProfileInfoProjectParticipantType.Text = "Поверенный (схема Поверенный - Доверитель)";
					break;
				case projectParticipantType.OPERATOR:
					tbProfileInfoProjectParticipantType.Text = "Оператор (схема Оператор - Подрядчик)";
					break;
				case projectParticipantType.TRUSTER:
					tbProfileInfoProjectParticipantType.Text = "Доверитель(схема Поверенный - Доверитель)";
					break;
				case projectParticipantType.CONTRACTOR:
					tbProfileInfoProjectParticipantType.Text = "Подрядчик (схема Оператор - Подрядчик)";
					break;
				default:
					tbProfileInfoProjectParticipantType.Text = profileInfo.projectParticipantType.ToString();
					break;
			}

			tbProfileInfoProjectCode.Text = profileInfo.projectCode.ToString();
			tbProfileInfoProjectName.Text = profileInfo.projectName;
		}
	}
}
