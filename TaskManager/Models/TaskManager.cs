using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TaskManager.Models
{
    public interface ITaskRepository : IDisposable
    {
        List<TaskModel> GetMyTaskList(string currentUserName);
        IDbSet<ApplicationUser> GetUserList();
        TaskModel GetTask(int? id);        
        void Create(TaskModel item);
        void Update(TaskModel item);
        void Delete(int id);
        void Save(TaskModel task, string managedUser);
        void SaveToTaskManagedUser(TaskModel task, string managedUser);
    }

    public class TaskRepository: ITaskRepository
    {
        private ApplicationDbContext db;
        public TaskRepository()
        {
            this.db = new ApplicationDbContext();
        }
        public List<TaskModel> GetMyTaskList(string currentUserName)
        {
            List<TaskModel> taskModels = new List<TaskModel>();
            if (db.Tasks != null)
            {
                ApplicationUser currentUser = db.Users.Where(u => u.Email == currentUserName).FirstOrDefault();
                taskModels = db.Tasks
                   .Where(t => t.Managers.Any(m => m.Id == currentUser.Id))
                   .Include(t => t.Autor)
                   .ToList();
            }            
            return taskModels;
        }

        public IDbSet<ApplicationUser> GetUserList()
        {
            return db.Users;
        }

        public TaskModel GetTask(int? id)
        {
            return db.Tasks.Find(id);
        }

        public void Create(TaskModel t)
        {
            db.Tasks.Add(t);
            db.SaveChanges();
        }

        public void Update(TaskModel t)
        {
            db.Entry(t).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void Save(TaskModel task, string managedUser)
        {
            ApplicationUser autor = db.Users.Where(u => u.Email == managedUser).FirstOrDefault();
            task.Autor = autor;
            task.Managers.Add(autor);
            db.Tasks.Add(task);
            db.SaveChanges();
        }

        public void SaveToTaskManagedUser(TaskModel task, string managedUser)
        {
            ApplicationUser sharedUser = db.Users.Find(managedUser);
            TaskModel editTask = db.Tasks.Find(task.TaskId);
            editTask.Managers.Add(sharedUser);
            db.Entry(editTask).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            TaskModel t = db.Tasks.Find(id);
            if (t != null)
                db.Tasks.Remove(t);
            db.SaveChanges();
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}