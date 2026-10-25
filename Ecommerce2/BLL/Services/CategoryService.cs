using ecommerce_application.MODELS.DB_MODELS;
using DAL.Services;
using DAL.DTO;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ecommerce_application.SERVICES
{
    public interface ICategoryService
    {
        Task<CategoryWithProductsVM> GetCategoryByIdAsync(int id);
        Task<IEnumerable<Categories>> GetAllCategoriesAsync();
        Task<int> AddCategoryAsync(Categories category);
        Task<bool> UpdateCategoryAsync(Categories category);
        Task<bool> DeleteCategoryAsync(int id);
        Task<List<CategoryVM>> GetActiveCategoriesAsync();
        Task<CategoryVM> GetCategoryByNameAsync(string name);
        Task DeactivateCategoryAsync(int id);
        Task ActivateCategoryAsync(int id);
        Task<CategoryWithProductCountVM> GetCategoryWithProductCountAsync(int id);
        Task<List<CategoryWithProductCountVM>> GetAllCategoriesWithProductCountAsync();
        Task<List<CategoryWithProductsVM>> GetAllCategoriesWithProductsAsync();
    }

    public class CategoryService : ICategoryService
    {
        private readonly CategoryDAO _categoryDAO;

        public CategoryService(CategoryDAO categoryDAO)
        {
            _categoryDAO = categoryDAO;
        }

        public async Task<CategoryWithProductsVM> GetCategoryByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Category ID must be greater than 0.");
            }

            var dto = await _categoryDAO.GetCategoryByIdAsync(id);

            if (dto == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            return dto;
        }

        public async Task<IEnumerable<Categories>> GetAllCategoriesAsync()
        {
            var dtos = await _categoryDAO.GetAllCategoriesAsync();

            if (dtos == null || !dtos.Any())
            {
                return new List<Categories>();
            }

            return dtos.Select(MapDtoToDb).ToList();
        }

        public async Task<int> AddCategoryAsync(Categories category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category), "Category cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                throw new ArgumentException("Category name is required.");
            }

            category.Name = category.Name.Trim();
            if (!string.IsNullOrWhiteSpace(category.Description))
            {
                category.Description = category.Description.Trim();
            }

            // Map to the CreateCategoryVM that CategoryDAO.CreateCategoryAsync expects
            var createModel = new CreateCategoryVM
            {
                Name = category.Name,
                Description = category.Description
                // map other required CreateCategoryVM properties here if any
            };

            var categoryId = await _categoryDAO.CreateCategoryAsync(createModel);

            if (categoryId <= 0)
            {
                throw new InvalidOperationException("Failed to create category.");
            }

            return categoryId;
        }

        public async Task<bool> UpdateCategoryAsync(Categories category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category), "Category cannot be null.");
            }

            if (category.Id <= 0)
            {
                throw new ArgumentException("Category ID must be greater than 0.");
            }

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                throw new ArgumentException("Category name is required.");
            }

            // Ensure category exists
            var existingDto = await _categoryDAO.GetCategoryByIdAsync(category.Id);
            if (existingDto == null)
            {
                throw new KeyNotFoundException($"Category with ID {category.Id} not found.");
            }

            // Trim values
            category.Name = category.Name.Trim();
            if (!string.IsNullOrWhiteSpace(category.Description))
            {
                category.Description = category.Description.Trim();
            }

            // Map to UpdateCategoryVM expected by DAO
            var updateModel = new UpdateCategoryVM
            {
                Name = category.Name,
                Description = category.Description
            };

            // DAO.UpdateCategoryAsync expects (int id, UpdateCategoryVM model) and returns Task
            await _categoryDAO.UpdateCategoryAsync(category.Id, updateModel);

            // If no exception thrown, consider update successful
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Category ID must be greater than 0.");
            }

            var dto = await _categoryDAO.GetCategoryByIdAsync(id);
            if (dto == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            // DAO.DeleteCategoryAsync returns Task; call and return true on success
            await _categoryDAO.DeleteCategoryAsync(id);
            return true;
        }

        // Mapping helpers
        private static Categories MapDtoToDb(CategoryVM dto)
        {
            if (dto == null) return null;
            return new Categories
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description
                // map other properties if Categories exposes them
            };
        }

        private static CategoryVM MapDbToDto(Categories dbModel)
        {
            if (dbModel == null) return null;
            return new CategoryVM
            {
                Id = dbModel.Id,
                Name = dbModel.Name,
                Description = dbModel.Description
                // map other properties if CategoryVM exposes them
            };
        }

        /// <summary>
        /// Get all active categories
        /// </summary>
        public async Task<List<CategoryVM>> GetActiveCategoriesAsync()
        {
            try
            {
                return await _categoryDAO.GetActiveCategoriesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Get category by name
        /// </summary>
        public async Task<CategoryVM> GetCategoryByNameAsync(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("Category name is required.");
                }
                return await _categoryDAO.GetCategoryByNameAsync(name);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Deactivate a category
        /// </summary>
        public async Task DeactivateCategoryAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("Category ID must be greater than 0.");
                }
                await _categoryDAO.DeactivateCategoryAsync(id);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Activate a category
        /// </summary>
        public async Task ActivateCategoryAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("Category ID must be greater than 0.");
                }
                await _categoryDAO.ActivateCategoryAsync(id);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Get category with product count
        /// </summary>
        public async Task<CategoryWithProductCountVM> GetCategoryWithProductCountAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("Category ID must be greater than 0.");
                }
                return await _categoryDAO.GetCategoryWithProductCountAsync(id);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Get all categories with product count
        /// </summary>
        public async Task<List<CategoryWithProductCountVM>> GetAllCategoriesWithProductCountAsync()
        {
            try
            {
                return await _categoryDAO.GetAllCategoriesWithProductCountAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Get all categories with their products
        /// </summary>
        public async Task<List<CategoryWithProductsVM>> GetAllCategoriesWithProductsAsync()
        {
            try
            {
                return await _categoryDAO.GetAllCategoriesWithProductsAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
