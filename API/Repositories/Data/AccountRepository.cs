﻿using API.Context;
using API.Models;
using API.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Repositories.Data
{
    public class AccountRepository
    {

        MyContext myContext;

        public AccountRepository(MyContext myContext)
        {
            this.myContext = myContext;
        }

        public List<ResponseLogin> Get()
        {
            var data = myContext.UserRole
                .Include(x => x.Role)
                .Include(x => x.User.Karyawan)
                .ToList();
            var dataKaryawan = new List<ResponseLogin>();
            foreach (var item in data)
            {
                var temp = new ResponseLogin()
                {
                    Id = item.Id,
                    FullName = item.User.Karyawan.Fullname,
                    Email = item.User.Karyawan.Email,
                    Alamat = item.User.Karyawan.Alamat,
                    Departemen = item.User.Karyawan.Departemen,
                    Telp = item.User.Karyawan.Telp,
                    Role = item.Role.Nama

                };
                dataKaryawan.Add(temp);
            }
            return dataKaryawan;
        }

        public Object Get(int id)
        {
            var karyawan = myContext.Karyawan.Find(id);
            var userrole = myContext.UserRole.Find(id);
            var role = myContext.Role.Find(userrole.Role_Id);
            var data = new ResponseLogin()
            {
                Id = karyawan.Id,
                FullName = karyawan.Fullname,
                Email = karyawan.Email,
                Alamat = karyawan.Alamat,
                Telp = karyawan.Telp,
                Departemen = karyawan.Departemen,
                Role = role.Nama

            };
            return data;
        }

        public List<Role> GetRoles()
        {
            var data = myContext.Role.ToList();
            return data;
        }
        public bool Delete(int id)
        {
            var data = myContext.Karyawan.Find(id);
            if (data == null)
            {
                return false;
            }
            myContext.Remove(data);
            if (myContext.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        public ResponseLogin Login(Login login)
        {

            var data = myContext.UserRole
                .Include(x => x.Role)
                .Include(x => x.User)
                .Include(x => x.User.Karyawan)
                .FirstOrDefault(x =>
                    x.User.Karyawan.Email.Equals(login.Email));
            if (data == null)
            {
                return null;
            }
            bool check = Hashing.PasswordVerify(login.Password, data.User.Password);
            if (check == true)
            {
                ResponseLogin responseLogin = new ResponseLogin()
                {
                    Id = data.User_Id,
                    FullName = data.User.Karyawan.Fullname,
                    Email = data.User.Karyawan.Email,
                    Role = data.Role.Nama,
                    Telp = data.User.Karyawan.Telp,
                    Alamat = data.User.Karyawan.Alamat

                };
                return responseLogin;
            }
            return null;
        }

        public bool Register(RegisterAccount registerAccount)
        {
            //
            if (registerAccount.Role.Equals(3))
            {
                registerAccount.Password = "@@@123456@@@";  //seharusnya karyawan tdk bisa akses aplikasi/ login
            }
            var transaction = myContext.Database.BeginTransaction(); 
            
            try
            {
                Karyawan dataKaryawan = new Karyawan
                {
                    //Id = registerAccount.Id,
                    Fullname = registerAccount.Fullname,
                    Email = registerAccount.Email,
                    Alamat = registerAccount.Alamat,
                    Telp = registerAccount.Telp,
                    Departemen = registerAccount.Departemen
                };
                myContext.Karyawan.Add(dataKaryawan);
                myContext.SaveChanges();

                User userData = new User
                {
                    Id = dataKaryawan.Id,
                    Password = Hashing.PasswordHashing(registerAccount.Password),
                    Karyawan = dataKaryawan

                };
                myContext.User.Add(userData);
                myContext.SaveChanges();

                UserRole userRoleDataa = new UserRole
                {
                    User_Id = dataKaryawan.Id,
                    Role_Id = registerAccount.Role

                };
                myContext.UserRole.Add(userRoleDataa);
                myContext.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
            return true;
        }



        public bool ChangePassword(ChangePassword changePassword)
        {
            var data = myContext.UserRole
                .Include(x => x.Role)
                .Include(x => x.User)
                .Include(x => x.User.Karyawan)
                .FirstOrDefault(x =>
                    x.User.Karyawan.Email.Equals(changePassword.login.Email));

            if (data == null)
            {
                return false;
            }
            if (!Hashing.PasswordVerify(changePassword.login.Password, data.User.Password))
            {
                return false;
            }
            User dataUser = myContext.User.Find(data.User_Id);
            dataUser.Password = Hashing.PasswordHashing(changePassword.newPassword);
            myContext.User.Update(dataUser);
            int result = myContext.SaveChanges();
            if (result == 0)
            {
                return false;
            }
            return true;
        }

        public bool UpdateAccount(UpdateAccount updateAccount)
        {
            var transaction = myContext.Database.BeginTransaction();
            try
            {
                var dataKaryawan = myContext.Karyawan.Find(updateAccount.Id);
                if (dataKaryawan == null)
                {
                    return false;
                }
                dataKaryawan.Fullname = updateAccount.Fullname;
                dataKaryawan.Alamat = updateAccount.Alamat;
                dataKaryawan.Telp = updateAccount.Telp;
                myContext.Karyawan.Update(dataKaryawan);
                myContext.SaveChanges();
                var dataRole = myContext.UserRole.Find(dataKaryawan.Id);
                dataRole.Role_Id = updateAccount.Role;
                myContext.UserRole.Update(dataRole);
                myContext.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
            return true;
        }



    }
}
