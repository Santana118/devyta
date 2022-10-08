﻿using API.Context;
using API.Models;
using API.Repositories.Interface;
using API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories.Data
{
    public class BarangRepository : IBarangRepository
    {
        MyContext myContext;

        public BarangRepository(MyContext myContext)
        {
            this.myContext = myContext;
        }
        public int Delete(int Id)
        {
            var data = Get(Id);
            if (data != null && data.Stok == 0)
            {
                myContext.Barang.Remove(data);
                var result = myContext.SaveChanges();
                return result;
            }   
            return -1;
        }

        public List<Barang> Get()
        {
            var data = myContext.Barang.ToList();
            return data;
        }

        public Barang Get(int id)
        {
            var data = myContext.Barang.Find(id);
            return data;
        }

        public int Post(BarangVM barang)
        {
            myContext.Barang.Add(new Barang { Nama = barang.Nama, Satuan = barang.Satuan });
            var result = myContext.SaveChanges();
            return result;
        }

        public int Put(Barang barang)
        {
            var data = Get(barang.Id);
            if (data == null)
                return -1;
            data.Nama = barang.Nama;
            data.Satuan = barang.Satuan;
            myContext.Barang.Update(data);
            var result = myContext.SaveChanges();
            return result;
        }
      
    }
}
