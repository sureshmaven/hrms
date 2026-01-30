using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Repository;

namespace HRMSApplication.Helpers
{
     
    public class PswdEncryptDecrypt
    {
        private ContextBase db = new ContextBase();
       
        public string Encrypt(string clearText)
        {
            string EncryptionKey = "TSCAB_HRMS@2018";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "TSCAB_HRMS@2018";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        //public string ChangePasswordEncrepty()
        //{
        //    string lmessage = "";
        //    var lemployees = db.Employes.ToList();
        //    foreach (var item in lemployees)
        //    {
        //        var lempid = item.EmpId;
        //        var lpassword = Encrypt(item.Password);
        //        var lemp = lemployees.Where(a => a.EmpId == lempid).FirstOrDefault();
        //        lemp.Password = lpassword;
        //        db.Entry(lemp).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //    return lmessage;
        //}
    }
}