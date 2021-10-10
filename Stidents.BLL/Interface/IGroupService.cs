﻿using Students.BLL.Interface.Base;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.Interface
{
    public interface IGroupService : IBaseService<Group>
    {
        Task<Group> GetAsync(int? id);
        Task StartGroup(int id);
        Task<IEnumerable<Group>> SearchAllAsync(string query);
    }
}