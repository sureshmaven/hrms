using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollModels
{
   public class CommonReportModel
    { 
        public int RowId { get; set; }
        public int SNo { get; set; }
        public string SlNo { get; set; }
        public string HRF { get; set; }
        public string column1 { get; set; }
        public string column2 { get; set; }
        public string column3 { get; set; }
        public string column4 { get; set; }
        public string column5 { get; set; }
        public string column6 { get; set; }
        public string column7 { get; set; }
        public string column8 { get; set; }
        public string column9 { get; set; }
        public string column10 { get; set; }
        public string column11 { get; set; }
        public string column12 { get; set; }
        public string column13 { get; set; }
        public string column14 { get; set; }
        public string column15 { get; set; }
        public string column16 { get; set; }
        public string column17 { get; set; }
        public string column18 { get; set; }
        public string column19 { get; set; }
        public string column20 { get; set; }
        public string column21 { get; set; }
        public string grpclmn { get; set; }
        public string grpclmn1 { get; set; }
        public string footer { get; set; }
        public string Total { get; set; }
        public string MedAlw { get; set; }
        public string SPLCashier { get; set; }
    }
    public class TsheetModel
    {
        public int RowId { get; set; }
        public int SNo { get; set; }
        public string SlNo { get; set; }
        public string HRF { get; set; }
        public string column1 { get; set; }
        public string column2 { get; set; }
        public string column3 { get; set; }
        public string column4 { get; set; }
        public string column5 { get; set; }
        public string column6 { get; set; }
        public string column7 { get; set; }
        public string column8 { get; set; }
        public string column9 { get; set; }
        public string column10 { get; set; }
        public string column11 { get; set; }
        public string column12 { get; set; }
        public string column13 { get; set; }
        public string column14 { get; set; }
        public string column15 { get; set; }
        public string column16 { get; set; }
        public string column17 { get; set; }
        public string column18 { get; set; }
        public string SPLCareTaker { get; set; }
        public string column20 { get; set; }
        public string column21 { get; set; }
        public string grpclmn { get; set; }
        public string grpclmn1 { get; set; }
        public string footer { get; set; }
        public string Total { get; set; }
        public string MedAlw { get; set; }
        public string SPLCashier { get; set; }
        public string SplIncr { get; set; }
        public string StagIncr { get; set; }
        public string AnlIncr { get; set; }
        public string Caib { get; set; }
        public string Jaib { get; set; }
        public string EmployeeTds { get; set; }
        public string LICPremium { get; set; }
        public string LossofPay { get; set; }
        public string SpecialPay { get; set; }
        public string SAdminAl { get; set; }
        public string TeachAlw { get; set; }
        public string WashAlw { get; set; }
        public string FixAlw { get; set; }
        public string DepuAlw { get; set; }
        public string FPA { get; set; }
        public string FPAHRA { get; set; }
        public string InterimAlw { get; set; }
        public string FPIIPAlw { get; set; }
        public string NPSGAlw{ get; set; }
        public string OfficiatingAlw{ get; set; }
        public string PPay { get; set; }
        public string Pqa { get; set; }
        public string ResAttenders { get; set; }
        public string FPIncentive  { get; set; }
        public string BrManager { get; set; }
        public string PetrolPaper { get; set; }
        public string PetrolPaper1 { get; set; }
        public string CEA { get; set; }
        public string Fest { get; set; }
        public string LTPFLoan { get; set; }
        public string INCENTIVE { get; set; }
        public string INCENTIVEDIFF { get; set; }
        public string LTCENCASHMENT { get; set; }
        public string SPLDriver { get; set; }
        public string SPLJamedar { get; set; }
        public string SPLKey { get; set; }
        public string SPLLiftOperator { get; set; }
        public string SPLNonPromotional { get; set; }
        public string SPLSplitDutyAwardstaff { get; set; }
        public string SPLTypist { get; set; }
        public string SPLWatchman { get; set; }
        public string SPLStenographer { get; set; }
        public string SPLBillAlw { get; set; }
        public string SPLDespatch { get; set; }
        public string SPLElectrician { get; set; }
        public string SPLDafter { get; set; }
        public string SPLCashCabin { get; set; }
        public string SPLTelephoneOperator { get; set; }
        public string SPLLibrary { get; set; }
        public string SPLArrearIncentive { get; set; }
        public string SPLConveyance { get; set; }
        public string SPLSplitDutyManagers { get; set; }
        public string SPLDuplicatingxeroxmachine { get; set; }
        public string SPLRecordroomasstallowance { get; set; }
        public string SPLRecordroomsubstaffall { get; set; }
        public string SPLReceptionistallowance { get; set; }
        public string SPLSplAlwACSTI { get; set; }
        public string SPLPersonalPay { get; set; }
        public string FACULTYAlw { get; set; }
        public string SPLIncentive { get; set; }

        public string  APCOBHFCLT { get;set;}
        public string APCOBHFCHO	{get;set;}
        public string VIJAYACoopSocietyDeduction { get;set;}
        public string VISAKHACoopSocietyDeduction { get;set;}
        public string GSLI { get;set;}
        public string OfficersAssnFund { get;set;}
        public string CadreOfficersAssnFund { get;set;}
        public string ClubSubscription { get;set;}
        public string UnionClubSubscription { get;set;}
        public string SCSTAssnLTSubscription { get;set;}
        public string SCSTAssnSTSubscription { get;set;}
        public string LTStaffBenefitFund { get;set;}
        public string VPFDeduction { get;set;}
        public string VPFPercentage { get;set;}
        public string LICPENSION { get;set;}
        public string JEEVANSURAKSHA { get;set;}
        public string HDFC { get;set;}
        public string CCAAP { get;set;}
        public string CCSHYD { get;set;}
        public string ABHLFHYD { get;set;}
        public string APCCADBEMPCCS { get;set;}
        public string APCOBEDLOANHNRBR { get;set;}
        public string APCOBEDLOANHO { get;set;}
        public string APSCBLTEmpAssn { get;set;}
        public string DRCCSVIZAG { get;set;}
        public string PRNJRCVLJUDGEVIZAG { get;set;}
        public string ADDLJRCVLJUDGEANUKU { get;set;}
        public string KADAPADCCB { get;set;}
        public string JRCVLJUDGESULLURPET { get;set;}
        public string KMLCOOPBANKVIZAG { get;set;}
        public string ESTTEXCESSHRAREC { get;set;}
        public string FESTIVALADVANCE { get;set;}
        public string ANDHRABANKRAMANTHPUR { get;set;}
        public string SRCIVILJUDGEKADAPA { get;set;}
        public string MEDICALADVANCERECOVERY { get;set;}
        public string XIXJRCVLJUDGECCCOURTHYDERABAD { get;set;}
        public string BANKSEMPASSNTELANGANA { get;set;}
        public string LICMACHILIPATNAM { get;set;}
        public string VEHICLELOANMACHILIPATNAM { get;set;}
        public string FESTIVALADVANCEMEDAK { get;set;}
        public string FESTADVMEDAK { get;set;}
        public string XIJRCVLJUDGECCCOURTSEC { get;set;}
        public string FESTIVALADVANCEELURU { get;set;}
        public string TELANGANAEMPUNION { get;set;}
        public string DCCBDEDUCTION { get;set;}
        public string COURTDEDUCTION { get;set;}
        public string LIFEINSURANCE { get;set;}
        public string TELANGANAOFFICERSASSN { get;set;}
        public string TSCABOFFICERSASSN { get;set;}
        public string MISCDEDUCTION { get;set;}
        public string PERSONALLOAN { get;set;}

    }

    public class misearnreportscolumns
    {
        public int RowId { get; set; }
        public int SNo { get; set; }
        public string column1 { get; set; }
        public string allowance_type { get; set; }
        public string dim_tid_ref_allow { get; set; }
        public string ABHLFHYD { get; set; }
        public string ANNUALINCREMENT { get; set; }
        public string APCCADBEMP { get; set; }
        public string APCOBEDLHNR { get; set; }
        public string APCOBEDLHO { get; set; }
        public string APCOBHFCHO { get; set; }
        public string APCOBHFCLT { get; set; }
        public string APSCBLTASSN { get; set; }
        public string ARR_GR_AMT { get; set; }
        public string ARREARGROSSAMOUNT { get; set; }
        public string BASIC { get; set; }
        public string BRMANAGERALLOWANCE { get; set; }
        public string BR_MGR { get; set; }
        public string CAIIB { get; set; }
        public string CAIIBINCREMENT { get; set; }
        public string CCA { get; set; }
        public string CCSAP { get; set; }
        public string CCSHYD { get; set; }
        public string CD2 { get; set; }
        public string CEOALLOWANCE { get; set; }
        public string CEOALLW { get; set; }
        public string CHILDRENEDUCATIONALLOWANCE { get; set; }
        public string CODINSPRM { get; set; }
        public string CODINSPRM1 { get; set; }
        public string CODPERKS { get; set; }
        public string COURT { get; set; }
        public string COURTAB { get; set; }
        public string COURTDED { get; set; }
        public string COURTHYD { get; set; }
        public string COURTKADAPA { get; set; }
        public string COURTSECBAD { get; set; }
        public string COURTSLRPET { get; set; }
        public string COURTTANUKU { get; set; }
        public string COURTVIZAG { get; set; }
        public string DA { get; set; }
        public string DCCB { get; set; }
        public string DEPU { get; set; }
        public string DEPUTATIONALLOWANCE { get; set; }
        public string DRCCSVIZAG { get; set; }
        public string EASSNSUB { get; set; }
        public string EDUC { get; set; }
        public string EMPLOYEETDS { get; set; }
        public string ENCASHMENT { get; set; }
        public string ESI { get; set; }
        public string ESTT { get; set; }
        public string EXGRATIA { get; set; }
        public string FA { get; set; }
        public string FACULTYALLOWANCE { get; set; }
        public string FAELURU { get; set; }
        public string FAMEDHAK { get; set; }
        public string FAONGOLE { get; set; }
        public string FCLTYALLW { get; set; }
        public string FEST { get; set; }
        public string FESTADVANCE { get; set; }
        public string FGPRM { get; set; }
        public string FIXEDALLOWANCE { get; set; }
        public string FIXEDPERSONALALLOWANCE { get; set; }
        public string FPINCENTIVERECOVERY { get; set; }
        public string FPAHRAALLOWANCE { get; set; }
        public string FPIIP { get; set; }
        public string FPIR { get; set; }
        public string FXDALLW { get; set; }
        public string GSLI { get; set; }
        public string HDCF { get; set; }
        public string HFC { get; set; }
        public string HL2 { get; set; }
        public string HL2A { get; set; }
        public string HL2BC { get; set; }
        public string HLADD { get; set; }
        public string HLCOM { get; set; }
        public string HLPLT { get; set; }
        public string HOUINSUPRE { get; set; }
        public string HOUPROPINS { get; set; }
        public string HOUS1 { get; set; }
        public string HOUS2 { get; set; }
        public string HOUS3 { get; set; }
        public string HOUSI { get; set; }
        public string HRA { get; set; }
        public string HRADIFFAUG10 { get; set; }
        public string HYDCOURT { get; set; }
        public string INCENTIVE { get; set; }
        public string INCENTIVEDIFF { get; set; }
        public string INCREMENT { get; set; }
        public string INTERESTONNSC { get; set; }
        public string INTERIM { get; set; }
        public string INTERIMALLOWANCE { get; set; }
        public string INTERIMRELIEF { get; set; }
        public string INTERMRELIEF { get; set; }
        public string JAIIB { get; set; }
        public string JAIIBINCREMENT { get; set; }
        public string JEEVAN { get; set; }
        public string KADAPADCCB { get; set; }
        public string KMLCOOPBANK { get; set; }
        public string LEAVEENCASHMENT { get; set; }
        public string LIC { get; set; }
        public string LICPREMIUM { get; set; }
        public string LICMACHILI { get; set; }
        public string LICPENSION { get; set; }
        public string LIFEINS { get; set; }
        public string LOANPERKS { get; set; }
        public string LOSSOFPAY { get; set; }
        public string LTPFLOAN { get; set; }
        public string LTASSNLEVY { get; set; }
        public string LTC { get; set; }
        public string LTCENCASHMENT { get; set; }
        public string LTCADVREC { get; set; }
        public string MARR { get; set; }
        public string MEDICAL { get; set; }
        public string MEDICALAID { get; set; }
        public string MEDICALALLOWANCE { get; set; }
        public string MEDICALADVANCE { get; set; }
        public string MISCDED { get; set; }
        public string MISCEARN { get; set; }
        public string MISCELLAN { get; set; }
        public string NPSGALLOWANCE { get; set; }
        public string NPSGA { get; set; }
        public string OFFASSN { get; set; }
        public string OFFASSNC { get; set; }
        public string OFFI { get; set; }
        public string OFFICIATINGALLOWANCE { get; set; }
        public string ONDAYSAL { get; set; }
        public string ONEDAYSAL { get; set; }
        public string OTWAGES { get; set; }
        public string PERNLON1 { get; set; }
        public string PERPAY { get; set; }
        public string PERQPAY { get; set; }
        public string PERS { get; set; }
        public string PERSONALPAY { get; set; }
        public string PERSONALQUALALLOWANCE { get; set; }
        public string PETROLPAPER { get; set; }
        public string PETROLPAPER1 { get; set; }
        public string PF { get; set; }
        public string PF5 { get; set; }
        public string PFHT1 { get; set; }
        public string PFHT2 { get; set; }
        public string PFLT1 { get; set; }
        public string PFLT2 { get; set; }
        public string PFLT3 { get; set; }
        public string PFLT4 { get; set; }
        public string PFPERKS { get; set; }
        public string PH { get; set; }
        public string PHONE { get; set; }
        public string PHYSICALLYHANDICAPPED { get; set; }
        public string RESATTENDERSALLOWANCE { get; set; }
        public string RESATTN { get; set; }
        public string SBF { get; set; }
        public string SBFLN { get; set; }
        public string SHIFTDUTYALLOWANCE { get; set; }
        public string SOCIE { get; set; }
        public string SP_ACSTI { get; set; }
        public string SP_ARREAR { get; set; }
        public string SP_BILLCOLL { get; set; }
        public string SP_CARETAKE { get; set; }
        public string SP_CASHCAD { get; set; }
        public string SP_CASHIER { get; set; }
        public string SP_CONVEY { get; set; }
        public string SP_DAFTARI { get; set; }
        public string SP_DESPATCH { get; set; }
        public string SP_DRIVER { get; set; }
        public string SP_ELEC { get; set; }
        public string SP_INCENTIVE { get; set; }
        public string SP_JAMEDAR { get; set; }
        public string SP_KEY { get; set; }
        public string SP_LIBRARY { get; set; }
        public string SP_LIFT { get; set; }
        public string SP_NONPROM { get; set; }
        public string SP_PERPAY { get; set; }
        public string SP_RECASST { get; set; }
        public string SP_RECEPTION { get; set; }
        public string SP_RECSUB { get; set; }
        public string SP_SD_AWARD { get; set; }
        public string SP_SD_MGR { get; set; }
        public string SP_SHFTDUTY { get; set; }
        public string SP_STENO { get; set; }
        public string SP_TELEPHONE { get; set; }
        public string SP_TYPIST { get; set; }
        public string SP_WATCHMAN { get; set; }
        public string SP_XEROX { get; set; }
        public string SPCLDA { get; set; }
        public string SPECIALINCREMENT { get; set; }
        public string SPECIALPAY { get; set; }
        public string SPLARREARINCENTIVE { get; set; }
        public string SPLBILLCOLLECTOR { get; set; }
        public string SPLCARETAKER { get; set; }
        public string SPLCASHCABIN { get; set; }
        public string SPLCASHIER { get; set; }
        public string SPLCONVEYANCE { get; set; }
        public string SPLDAFTAR { get; set; }
        public string SPLDAFTER { get; set; }
        public string SPLDESPATCH { get; set; }
        public string SPLDRIVER { get; set; }
        public string SPLDUPLICATINGXEROXMACHINE { get; set; }
        public string SPLELECTRICIAN { get; set; }
        public string SPLINCENTIVE { get; set; }
        public string SPLJAMEDAR { get; set; }
        public string SPLKEY { get; set; }
        public string SPLLIBRARY { get; set; }
        public string SPLLIFTOPERATOR { get; set; }
        public string SPLNONPROMOTIONAL { get; set; }
        public string SPLPERSONALPAY { get; set; }
        public string SPLRECEPTIONISTALLOWANCE { get; set; }
        public string SPLRECORDROOMASSTALLOWANCE { get; set; }
        public string SPLRECORDROOMSUBSTAFFALL { get; set; }
        public string SPLRECORDROOMUBSTAFFALLOWANCE { get; set; }
        public string SPLSPLALWACSTI { get; set; }
        public string SPLSPLITDUTYMANAGERS { get; set; }
        public string SPLSPLITDUTYAWARDSTAFF { get; set; }
        public string SPLSTENOGRAPHER { get; set; }
        public string SPLTELEPHONEOPERATOR { get; set; }
        public string SPLTYPIST { get; set; }
        public string SPLWATCHMAN { get; set; }
        public string SPLALLOW { get; set; }
        public string SPLINCR { get; set; }
        public string SPLPAY { get; set; }
        public string STAGALLOW { get; set; }
        public string STAGNATIONINCREMENTS { get; set; }
        public string SUBCLUB { get; set; }
        public string SUBLT { get; set; }
        public string SUBST { get; set; }
        public string SUBUNION { get; set; }
        public string SYSADMN { get; set; }
        public string SYSTEMADMINISTRATORALLOWANCE { get; set; }
        public string TDS { get; set; }
        public string TEACH { get; set; }
        public string TEACHINGALLOWANCE { get; set; }
        public string TELANGANAINCREMENT { get; set; }
        public string TGASSN { get; set; }
        public string TGUNION { get; set; }
        public string TOFA { get; set; }
        public string TSCABOA { get; set; }
        public string UNIONLEVY { get; set; }
        public string VEH2W { get; set; }
        public string VEH4W { get; set; }
        public string VEHINS { get; set; }
        public string VEHMACHILI { get; set; }
        public string VIJAYA { get; set; }
        public string VISAKHA { get; set; }
        public string VPF { get; set; }
        public string WASHALLW { get; set; }
        public string WASHINGALLOWANCE { get; set; }

    }
    public class TDSReport
    {
        public string column1 { get; set; }
        public string column2 { get; set; }
        public string column3 { get; set; }
        public string column4 { get; set; }
    }
    public class MISReportModel
    {
        public int RowId { get; set; }
        public int SNo { get; set; }
        public string SlNo { get; set; }
        public string HRF { get; set; }
        public string column1 { get; set; }
        public string column2 { get; set; }
        public string column3 { get; set; }
        public string column4 { get; set; }
        public string column5 { get; set; }
        public string column6 { get; set; }
        public string column7 { get; set; }
        public string column8 { get; set; }
        public string column9 { get; set; }
        public string column10 { get; set; }
        public string column11 { get; set; }
        public string column12 { get; set; }
        public string column13 { get; set; }
        public string column14 { get; set; }
        public string column15 { get; set; }
        public string column16 { get; set; }
        public string column17 { get; set; }
        public string column18 { get; set; }
        public string column19 { get; set; }
        public string column20 { get; set; }
        public string column21 { get; set; }
        public string column22 { get; set; }
        public string column23 { get; set; }
        public string column24 { get; set; }
        public string column25 { get; set; }
        public string column26 { get; set; }
        public string column27 { get; set; }
        public string column28 { get; set; }
        public string column29 { get; set; }
        public string column30 { get; set; }
        public string column31 { get; set; }
        public string column32 { get; set; }
        public string column33 { get; set; }
        public string column34 { get; set; }
        public string column35 { get; set; }
        public string column36 { get; set; }
        public string column37 { get; set; }
        public string column38 { get; set; }
        public string column39 { get; set; }
        public string column40 { get; set; }
        public string column41 { get; set; }
        public string column42 { get; set; }
        public string column43 { get; set; }
        public string column44 { get; set; }
        public string column45 { get; set; }
        public string column46 { get; set; }
        public string column47 { get; set; }
        public string column48 { get; set; }
        public string column49 { get; set; }
        public string column50 { get; set; }
        public string column51 { get; set; }
        public string column52 { get; set; }
        public string column53 { get; set; }
        public string column54 { get; set; }
        public string column55 { get; set; }
        public string column56 { get; set; }
        public string column57 { get; set; }
        public string column58 { get; set; }
        public string column59 { get; set; }
        public string column60 { get; set; }
        public string column61 { get; set; }
        public string column62 { get; set; }
        public string column63 { get; set; }
        public string column64 { get; set; }
        public string column65 { get; set; }
        public string column66 { get; set; }
        public string column67 { get; set; }
        public string column68 { get; set; }
        public string column69 { get; set; }
        public string column70 { get; set; }
        public string column71 { get; set; }
        public string column72 { get; set; }
        public string column73 { get; set; }
        public string column74 { get; set; }
        public string column75 { get; set; }
        public string column76 { get; set; }
        public string column77 { get; set; }
        public string column78 { get; set; }
        public string column79 { get; set; }
        public string column80 { get; set; }
        public string grpclmn { get; set; }
        public string grpclmn1 { get; set; }
        public string footer { get; set; }
        public string Total { get; set; }
        public string MedAlw { get; set; }
        public string SPLCashier { get; set; }
    }
}
