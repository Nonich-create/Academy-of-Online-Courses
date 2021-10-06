﻿using Microsoft.Extensions.Logging;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq;
using System.Threading.Tasks;
using Students.DAL.Enum;

namespace Students.BLL.Services
{

    public class LessonService : ILessonService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public LessonService(UnitOfWork unitOfWork, ILogger<Lesson> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        
        public async Task<bool> CheckRecord(int CourseId, int NumberLesson)
        {
               if (await _unitOfWork.LessonRepository.SearchAsync($"CourseId = {CourseId} and NumberLesson = {NumberLesson}") == null)
            return true;
            return false;
        }
        
        public async Task CreateAsync(Lesson item)
        {
            try 
            {
                await _unitOfWork.LessonRepository.CreateAsync(item);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Урок создан");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка создания урока");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                Lesson lesson = await GetAsync(id);
                if (lesson != null)
                {
                    await _unitOfWork.LessonRepository.DeleteAsync(id);
                    _logger.LogInformation(id, "Урок удален");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления урока");
            }
        }

        public async Task<bool> ExistsAsync(int id) => await _unitOfWork.LessonRepository.ExistsAsync(id);
        
        public async Task<IEnumerable<Lesson>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Выполнения получения списка уроков");
                return await _unitOfWork.LessonRepository.GetAllAsync(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получение списка уроков");
                return Enumerable.Empty<Lesson>();
            }
        }

        public async Task<Lesson> GetAsync(int id)
        {
            try
            {
                _logger.LogInformation("Получение урока");
                return await _unitOfWork.LessonRepository.GetAsync(id); 
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получение урока");
                return null;
            }
        }

        public async Task<Lesson> Update(Lesson item)
        {
            try
            {
                var lesson = await _unitOfWork.LessonRepository.Update(item);
                _logger.LogInformation("Урок изменен");
                await _unitOfWork.SaveAsync();
                return lesson;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования урока");
                return item;
            }
        }

        public async Task<Lesson> SearchAsync(string query)
        {
            try
            {
                _logger.LogInformation("Поиск урока");
                return await _unitOfWork.LessonRepository.SearchAsync(query);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка поиска урока");
                return null;
            }
        }

        public async Task<int> GetCount(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
            {
                return (await _unitOfWork.LessonRepository.GetAllAsync()).Count();
            }
            return (await SearchAllAsync(searchString, searchParametr)).Count();
        }

        public async Task<IEnumerable<Lesson>> GetPaginatedResult(int currentPage, int pageSize = 10)
        {
            return (await _unitOfWork.LessonRepository.GetAllAsync())
                .OrderBy(l => l.NumberLesson).Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        public async Task<IEnumerable<Lesson>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Lesson>();
            return (await _unitOfWork.LessonRepository.GetAllAsync()).AsQueryable()
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString);
        }

        public async Task<IEnumerable<Lesson>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Lesson>();
            return (await _unitOfWork.LessonRepository.GetAllAsync()).AsQueryable()
                .OrderBy(l => l.NumberLesson)
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString)
                .Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        public async Task<IEnumerable<Lesson>> IndexView(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            if (!String.IsNullOrEmpty(searchString) && searchParametr != EnumSearchParameters.None)
            {
                return await SearchAllAsync(searchString, searchParametr, currentPage, pageSize);
            }
            return await GetPaginatedResult(currentPage, pageSize);
        }

        public async Task<IEnumerable<Lesson>> IndexView(int courseId, string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            if (!String.IsNullOrEmpty(searchString) && searchParametr != EnumSearchParameters.None)
            {
                return (await SearchAllAsync(searchString, searchParametr, currentPage, pageSize)).Where(l => l.CourseId == courseId);
            }
            return (await GetPaginatedResult(currentPage, pageSize)).Where(l => l.CourseId == courseId); ;
        }
    }
}
