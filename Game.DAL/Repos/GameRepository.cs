using Game.DAL.EF;
using Game.DAL.EF.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

// ১. নেমস্পেস কনফ্লিক্ট এড়াতে অ্যালিয়াস (Alias) ব্যবহার করা হলো
using GameClass = Game.DAL.EF.Tables.Game;

namespace Game.DAL.Repos
{
    public class GameRepository
    {
        private readonly GameSpdbContext db;

        public GameRepository(GameSpdbContext db)
        {
            this.db = db;
        }

        // নতুন গেম তৈরি করা
        public bool Create(GameClass b)
        {
            db.Games.Add(b);
            return db.SaveChanges() > 0;
        }

        // সব গেমের লিস্ট নেওয়া (ল্যান্ডিং পেজের জন্য এটি কল হবে)
        public List<GameClass> Get()
        {
            // db.Game() পরিবর্তন করে সঠিক নিয়মে db.Games.ToList() করা হলো
            return db.Games.ToList();
        }

        // আইডি দিয়ে নির্দিষ্ট গেম খোঁজা
        public GameClass? Get(int id)
        {
            // Category টেবিল বা কলাম না থাকায় .Include() অংশটি বাদ দেওয়া হয়েছে
            return db.Games.FirstOrDefault(b => b.Id == id);
        }

        // গেমের তথ্য আপডেট করা
        public bool Update(GameClass b)
        {
            var exobj = Get(b.Id);

            if (exobj == null)
            {
                return false;
            }

            db.Entry(exobj).CurrentValues.SetValues(b);
            return db.SaveChanges() > 0;
        }

        // গেম ডিলিট করা
        public bool Delete(int id)
        {
            var exobj = Get(id);
            if (exobj == null)
            {
                return false;
            }

            // IsActive না থাকায় সরাসরি ডাটাবেজ থেকে মুছে ফেলার কোড লেখা হলো
            db.Games.Remove(exobj);
            return db.SaveChanges() > 0;
        }
    }
}