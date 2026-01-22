using PoliceDB.Core.Models;
using PoliceDB.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PoliceDB.BLL.Services
{
    public class MockPendingChangeRepository : IPendingChangeRepository
    {
        private readonly List<PendingChange> _pendingChanges = new();

        public void Add(PendingChange change)
        {
            change.Id = Guid.NewGuid().ToString();
            _pendingChanges.Add(change);
        }

        public void Delete(string id)
        {
            var change = _pendingChanges.FirstOrDefault(c => c.Id == id);
            if (change != null)
            {
                _pendingChanges.Remove(change);
            }
        }

        public List<PendingChange> GetAll()
        {
            return _pendingChanges;
        }

        public PendingChange? GetById(string id)
        {
            return _pendingChanges.FirstOrDefault(c => c.Id == id);
        }

        public List<PendingChange> GetByStatus(ChangeStatus status)
        {
            return _pendingChanges.Where(c => c.Status == status).ToList();
        }

        public List<PendingChange> GetByUserId(string userId)
        {
            return _pendingChanges.Where(c => c.RequestedByUserId == userId).ToList();
        }

        public void Update(PendingChange change)
        {
            var existing = GetById(change.Id);
            if (existing != null)
            {
                existing.Status = change.Status;
                existing.ApprovedByUserId = change.ApprovedByUserId;
                existing.ApprovedDate = change.ApprovedDate;
                existing.Comment = change.Comment;
            }
        }
    }
}