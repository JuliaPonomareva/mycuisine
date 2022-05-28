using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyCuisine.Data.Web.Models;
using MyCuisine.Web.Constants;
using MyCuisine.Web.Data;
using MyCuisine.Web.Extensions;
using MyCuisine.Web.Models;

namespace MyCuisine.Web.Controllers
{
    [Authorize(Policy = AuthConstants.AdminPolicy)]
    public class AdminController : Controller
    {
        private readonly ApplicationContext _dbContext;
        private readonly ILogger<AdminController> _logger;
        private readonly AppSettings _appSettings;
        private readonly IWebHostEnvironment _environment;

        public AdminController(ApplicationContext dbContext, ILogger<AdminController> logger,
            AppSettings appSettings, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _logger = logger;
            _appSettings = appSettings;
            _environment = environment;
        }

        #region CookingStep
        [HttpGet("Admin/Recipes/{recipeId}/CookingSteps")]
        public async Task<IActionResult> RecipeCookingSteps(int recipeId)
        {
            var queryable = _dbContext.CookingSteps.AsNoTracking().Where(s => s.RecipeId == recipeId).AsQueryable();
            var total = await queryable.CountAsync();
            var entries = await queryable
                .Select(s => new CookingStepsViewModel.CookingStepViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Image = s.Image,
                    OrderNumber = s.OrderNumber
                })
                .ToListAsync();

            return View(new ViewModel<CookingStepsViewModel>
            {
                Data = new CookingStepsViewModel
                {
                    RecipeId = recipeId,
                    Entries = entries.OrderBy(s => s.OrderNumber).ToList()
                }
            });
        }

        private ViewModel<CookingStepCreateViewModel> GetCookingStepCreateViewModel(
            int recipeId, CookingStepCreateViewModel.FormModel form = null)
        {
            return new ViewModel<CookingStepCreateViewModel>
            {
                Data = new CookingStepCreateViewModel
                {
                    RecipeId = recipeId,
                    Form = form
                }
            };
        }

        [HttpGet("Admin/Recipes/{recipeId}/CookingStepCreate")]
        public IActionResult RecipeCookingStepCreate(int recipeId)
        {
            var model = GetCookingStepCreateViewModel(recipeId);
            return View(model);
        }

        [HttpPost("Admin/Recipes/{recipeId}/CookingStepCreate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecipeCookingStepCreate(int recipeId, ViewModel<CookingStepCreateViewModel> model)
        {
            if (model?.Data?.Form == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                model.Data.Form.Name = model.Data.Form.Name.ToLower();
                var entryWithSameStep = await _dbContext.CookingSteps
                    .AnyAsync(s => s.RecipeId == recipeId && s.Name == model.Data.Form.Name);

                if (!entryWithSameStep)
                {
                    string image = null;
                    if (model.Data.Form.Image != null)
                    {
                        if (model.Data.Form.Image.ContentType != "image/png"
                            && model.Data.Form.Image.ContentType != "image/jpeg")
                        {
                            HttpContext.SetError($"Разрешенный формат картинок: png, jpeg.");

                            model = GetCookingStepCreateViewModel(recipeId, model.Data.Form);
                            return View(model);
                        }
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.Data.Form.Image.FileName)}";
                        image = $"/img/steps/{fileName}";

                        using (var stream = System.IO.File.Create(Path.Join(_environment.WebRootPath, image)))
                        {
                            await model.Data.Form.Image.CopyToAsync(stream);
                        }
                    }

                    var entry = new CookingStep
                    {
                        Name = model.Data.Form.Name,
                        Description = model.Data.Form.Description,
                        OrderNumber = model.Data.Form.OrderNumber,
                        Image = image,
                        RecipeId = recipeId,
                        DateCreated = DateTimeOffset.Now,
                        DateModified = DateTimeOffset.Now
                    };

                    await _dbContext.CookingSteps.AddAsync(entry);
                    await _dbContext.SaveChangesAsync();

                    return LocalRedirect($"/Admin/Recipes/{recipeId}/CookingSteps");
                }
                else
                {
                    HttpContext.SetError($"Рецепт уже содержит шаг с названием {model.Data.Form.Name}.");
                }
            }

            model = GetCookingStepCreateViewModel(recipeId, model.Data.Form);
            return View(model);
        }

        private ViewModel<CookingStepUpdateViewModel> GetCookingStepUpdateViewModel(
            int recipeId, CookingStep entry, CookingStepUpdateViewModel.FormModel form = null)
        {
            return new ViewModel<CookingStepUpdateViewModel>
            {
                Data = new CookingStepUpdateViewModel
                {
                    RecipeId = recipeId,
                    Image = entry.Image,
                    Form = form ?? new CookingStepUpdateViewModel.FormModel
                    {
                        Name = entry.Name,
                        Description = entry.Description,
                        OrderNumber = entry.OrderNumber,
                        RemoveImage = false
                    }
                }
            };
        }

        [HttpGet("Admin/Recipes/{recipeId}/CookingSteps/{id}")]
        public async Task<IActionResult> RecipeCookingStepUpdate(int recipeId, int id)
        {
            var entry = await _dbContext.CookingSteps.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (entry == null)
            {
                return NotFound();
            }
            var model = GetCookingStepUpdateViewModel(recipeId, entry);
            return View(model);
        }

        [HttpPost("Admin/Recipes/{recipeId}/CookingSteps/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecipetCookingStepUpdate(int recipeId, int id,
            ViewModel<CookingStepUpdateViewModel> model)
        {
            if (model?.Data?.Form == null)
            {
                return BadRequest();
            }

            var entry = await _dbContext.CookingSteps.FirstOrDefaultAsync(s => s.Id == id);
            if (entry == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                model.Data.Form.Name = model.Data.Form.Name.ToLower();
                var entryWithSameStep = await _dbContext.CookingSteps
                    .AnyAsync(s => s.RecipeId == recipeId
                        && s.Name == model.Data.Form.Name && s.Id != id);

                if (!entryWithSameStep)
                {
                    string image = entry.Image;
                    if (model.Data.Form.Image != null)
                    {
                        if (model.Data.Form.Image.ContentType != "image/png"
                            && model.Data.Form.Image.ContentType != "image/jpeg")
                        {
                            HttpContext.SetError($"Разрешенный формат картинок: png, jpeg.");
                            model = GetCookingStepUpdateViewModel(recipeId, entry, model.Data.Form);
                            return View(model);
                        }

                        if (!string.IsNullOrWhiteSpace(image))
                        {
                            System.IO.File.Delete(Path.Join(_environment.WebRootPath, image));
                        }

                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.Data.Form.Image.FileName)}";
                        image = $"/img/steps/{fileName}";

                        using (var stream = System.IO.File.Create(Path.Join(_environment.WebRootPath, image)))
                        {
                            await model.Data.Form.Image.CopyToAsync(stream);
                        }

                    }
                    else if (model.Data.Form.RemoveImage && !string.IsNullOrWhiteSpace(image))
                    {
                        System.IO.File.Delete(Path.Join(_environment.WebRootPath, image));
                        image = null;
                    }

                    entry.Name = model.Data.Form.Name;
                    entry.Description = model.Data.Form.Description;
                    entry.OrderNumber = model.Data.Form.OrderNumber;
                    entry.Image = image;
                    entry.RecipeId = recipeId;
                    entry.DateModified = DateTimeOffset.Now;

                    await _dbContext.SaveChangesAsync();

                    return LocalRedirect($"/Admin/Recipes/{recipeId}/CookingSteps");
                }
                else
                {
                    HttpContext.SetError($"Рецепт уже содержит шаг с названием {model.Data.Form.Name}.");
                }
            }

            model = GetCookingStepUpdateViewModel(recipeId, entry, model.Data.Form);
            return View(model);
        }

        [HttpPost("Admin/Recipes/{recipeId}/CookingSteps/{id}/Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecipetCookingStepRemove(int recipeId, int id)
        {
            var entry = await _dbContext.CookingSteps.FirstOrDefaultAsync(s => s.Id == id);
            if (entry != null)
            {
                _dbContext.CookingSteps.Remove(entry);
                await _dbContext.SaveChangesAsync();
            }
            return LocalRedirect($"/Admin/Recipes/{recipeId}/CookingSteps");
        }
        #endregion

        #region RecipeItem
        [HttpGet("Admin/Recipes/{recipeId}/Items")]
        public async Task<IActionResult> RecipeItems(int recipeId)
        {
            var queryable = _dbContext.RecipeItems.AsNoTracking().Where(s => s.RecipeId == recipeId).AsQueryable();
            var total = await queryable.CountAsync();
            var entries = await queryable
                .Select(s => new RecipeItemsViewModel.RecipeItemViewModel
                {
                    Id = s.Id,
                    Ingredient = s.Ingredient.Name,
                    IsMain = s.IsMain,
                    OrderNumber = s.OrderNumber,
                    Quantity = s.Quantity,
                    QuantityType = s.QuantityType.Name
                })
                .ToListAsync();

            return View(new ViewModel<RecipeItemsViewModel>
            {
                Data = new RecipeItemsViewModel
                {
                    RecipeId = recipeId,
                    Entries = entries.OrderBy(s => s.OrderNumber).ToList()
                }
            });
        }

        private async Task<ViewModel<RecipeItemCreateViewModel>> GetRecipeItemCreateViewModel(
            int recipeId, RecipeItemCreateViewModel.FormModel form = null)
        {
            var ingredients = await _dbContext.Ingredients.Where(s => s.IsActive)
                .Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToListAsync();

            var quantityTypes = await _dbContext.QuantityTypes.Where(s => s.IsActive)
                .Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToListAsync();

            return new ViewModel<RecipeItemCreateViewModel>
            {
                Data = new RecipeItemCreateViewModel
                {
                    RecipeId = recipeId,
                    Ingredients = ingredients.OrderBy(s => s.Value).ToList(),
                    QuantityTypes = quantityTypes.OrderBy(s => s.Value).ToList(),
                    Form = form
                }
            };
        }

        [HttpGet("Admin/Recipes/{recipeId}/ItemCreate")]
        public async Task<IActionResult> RecipeItemCreate(int recipeId)
        {
            var model = await GetRecipeItemCreateViewModel(recipeId);
            return View(model);
        }

        [HttpPost("Admin/Recipes/{recipeId}/ItemCreate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecipeItemCreate(int recipeId, ViewModel<RecipeItemCreateViewModel> model)
        {
            if (model?.Data?.Form == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var entryWithSameIngredient = await _dbContext.RecipeItems
                    .AnyAsync(s => s.RecipeId == recipeId && s.IngredientId == model.Data.Form.IngredientId);

                if (!entryWithSameIngredient)
                {
                    var entry = new RecipeItem
                    {
                        Quantity = model.Data.Form.Quantity,
                        IsMain = model.Data.Form.IsMain,
                        OrderNumber = model.Data.Form.OrderNumber,
                        IngredientId = model.Data.Form.IngredientId,
                        QuantityTypeId = model.Data.Form.QuantityTypeId,
                        RecipeId = recipeId,
                        DateCreated = DateTimeOffset.Now,
                        DateModified = DateTimeOffset.Now
                    };

                    await _dbContext.RecipeItems.AddAsync(entry);
                    await _dbContext.SaveChangesAsync();

                    return LocalRedirect($"/Admin/Recipes/{recipeId}/Items");
                }
                else
                {
                    HttpContext.SetError($"Рецепт уже содержит этот ингредиент.");
                }
            }

            model = await GetRecipeItemCreateViewModel(recipeId, model.Data.Form);
            return View(model);
        }

        private async Task<ViewModel<RecipeItemUpdateViewModel>> GetRecipeItemUpdateViewModel(
            int recipeId, RecipeItem entry, RecipeItemUpdateViewModel.FormModel form = null)
        {
            var ingredients = await _dbContext.Ingredients.Where(s => s.IsActive)
                .Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToListAsync();

            var quantityTypes = await _dbContext.QuantityTypes.Where(s => s.IsActive)
                .Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToListAsync();

            return new ViewModel<RecipeItemUpdateViewModel>
            {
                Data = new RecipeItemUpdateViewModel
                {
                    RecipeId = recipeId,
                    Ingredients = ingredients.OrderBy(s => s.Value).ToList(),
                    QuantityTypes = quantityTypes.OrderBy(s => s.Value).ToList(),
                    Form = form ?? new RecipeItemUpdateViewModel.FormModel
                    {
                        IngredientId = entry.IngredientId,
                        IsMain = entry.IsMain,
                        Quantity = entry.Quantity,
                        OrderNumber = entry.OrderNumber,
                        QuantityTypeId = entry.QuantityTypeId
                    }
                }
            };
        }

        [HttpGet("Admin/Recipes/{recipeId}/Items/{id}")]
        public async Task<IActionResult> RecipeItemUpdate(int recipeId, int id)
        {
            var entry = await _dbContext.RecipeItems.AsNoTracking()
                .Include(s => s.Ingredient).Include(s => s.QuantityType)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (entry == null)
            {
                return NotFound();
            }
            var model = await GetRecipeItemUpdateViewModel(recipeId, entry);
            return View(model);
        }

        [HttpPost("Admin/Recipes/{recipeId}/Items/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecipetItemUpdate(int recipeId, int id, ViewModel<RecipeItemUpdateViewModel> model)
        {
            if (model?.Data?.Form == null)
            {
                return BadRequest();
            }

            var entry = await _dbContext.RecipeItems.FirstOrDefaultAsync(s => s.Id == id);
            if (entry == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var entryWithSameIngredient = await _dbContext.RecipeItems
                    .AnyAsync(s => s.RecipeId == recipeId
                        && s.IngredientId == model.Data.Form.IngredientId && s.Id != id);

                if (!entryWithSameIngredient)
                {
                    entry.Quantity = model.Data.Form.Quantity;
                    entry.IsMain = model.Data.Form.IsMain;
                    entry.OrderNumber = model.Data.Form.OrderNumber;
                    entry.IngredientId = model.Data.Form.IngredientId;
                    entry.QuantityTypeId = model.Data.Form.QuantityTypeId;
                    entry.DateModified = DateTimeOffset.Now;

                    await _dbContext.SaveChangesAsync();

                    return LocalRedirect($"/Admin/Recipes/{recipeId}/Items");
                }
                else
                {
                    HttpContext.SetError($"Рецепт уже содержит этот ингредиент.");
                }
            }

            model = await GetRecipeItemUpdateViewModel(recipeId, entry, model.Data.Form);
            return View(model);
        }

        [HttpPost("Admin/Recipes/{recipeId}/Items/{id}/Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecipetItemRemove(int recipeId, int id)
        {
            var entry = await _dbContext.RecipeItems.FirstOrDefaultAsync(s => s.Id == id);
            if (entry != null)
            {
                _dbContext.RecipeItems.Remove(entry);
                await _dbContext.SaveChangesAsync();
            }
            return LocalRedirect($"/Admin/Recipes/{recipeId}/Items");
        }
        #endregion

        #region Recipe
        [HttpGet]
        public async Task<IActionResult> Recipes([FromQuery] int offset = 0, [FromQuery] int? limit = null,
            [FromQuery] string search = null, [FromQuery] bool showAll = false)
        {
            limit = limit ?? _appSettings.PageSize;
            var queryable = _dbContext.Recipes.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                queryable = queryable.Where(s => s.Name.Contains(search)
                    || s.CuisineType.Name.Contains(search)
                    || s.DishType.Name.Contains(search)
                    || s.RecipesOtherProperties.Any(x => x.OtherProperty.Name.Contains(search))
                    || s.RecipeItems.Any(x => x.Ingredient.Name.Contains(search)
                        || x.QuantityType.Name.Contains(search))
                    || s.CookingSteps.Any(x => x.Name.Contains(search)));
            }
            if (!showAll)
            {
                queryable = queryable.Where(s => s.IsActive);
            }
            var total = await queryable.CountAsync();
            var entries = await queryable
                .OrderBy(s => s.Name)
                .Skip(offset).Take(limit.Value)
                .Select(s => new RecipesViewModel.RecipeViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Image = s.Image,
                    PersonsCount = s.PersonsCount,
                    Rate = s.Rate,
                    Votes = s.Votes,
                    IsActive = s.IsActive,
                    CuisineType = s.CuisineType.Name,
                    DishType = s.DishType.Name,
                    ItemsCount = s.RecipeItems.Count,
                    StepsCount = s.CookingSteps.Count
                })
                .ToListAsync();

            return View(new ViewModel<RecipesViewModel>
            {
                Data = new RecipesViewModel
                {
                    Entries = entries
                },
                Paging = new Paging(total, offset, limit.Value, (o, l) => $"/Admin/Recipes?offset={o}&limit={l}&search={search}&showAll={showAll}")
            });
        }

        private async Task<ViewModel<RecipeCreateViewModel>> GetRecipeCreateViewModel(RecipeCreateViewModel.FormModel form = null)
        {
            var dishTypes = await _dbContext.DishTypes.Where(s => s.IsActive)
                .Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToListAsync();

            var cuisineTypes = await _dbContext.CuisineTypes.Where(s => s.IsActive)
                .Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToListAsync();

            var otherProperties = await _dbContext.OtherProperties.Where(s => s.IsActive)
                .Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToListAsync();

            return new ViewModel<RecipeCreateViewModel>
            {
                Data = new RecipeCreateViewModel
                {
                    DishTypes = dishTypes.OrderBy(s => s.Value).ToList(),
                    CuisineTypes = cuisineTypes.OrderBy(s => s.Value).ToList(),
                    OtherProperties = otherProperties.OrderBy(s => s.Value).ToList(),
                    Form = form
                }
            };
        }

        [HttpGet("Admin/RecipeCreate")]
        public async Task<IActionResult> RecipeCreate()
        {
            var model = await GetRecipeCreateViewModel();
            return View(model);
        }

        [HttpPost("Admin/RecipeCreate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecipeCreate(ViewModel<RecipeCreateViewModel> model)
        {
            if (model?.Data?.Form == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                model.Data.Form.Name = model.Data.Form.Name.ToLower();
                var entryWithSameName = await _dbContext.Recipes.AnyAsync(s => s.Name == model.Data.Form.Name);
                if (!entryWithSameName)
                {
                    string image = null;
                    if (model.Data.Form.Image != null)
                    {
                        if (model.Data.Form.Image.ContentType != "image/png"
                            && model.Data.Form.Image.ContentType != "image/jpeg")
                        {
                            HttpContext.SetError($"Разрешенный формат картинок: png, jpeg.");

                            model = await GetRecipeCreateViewModel(model.Data.Form);
                            return View(model);
                        }
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.Data.Form.Image.FileName)}";
                        image = $"/img/recipes/{fileName}";

                        using (var stream = System.IO.File.Create(Path.Join(_environment.WebRootPath, image)))
                        {
                            await model.Data.Form.Image.CopyToAsync(stream);
                        }

                    }
                    var entry = new Recipe
                    {
                        Name = model.Data.Form.Name,
                        Description = model.Data.Form.Description,
                        Image = image,
                        PersonsCount = model.Data.Form.PersonsCount,
                        DishTypeId = model.Data.Form.DishTypeId,
                        CuisineTypeId = model.Data.Form.CuisineTypeId,
                        IsActive = model.Data.Form.IsActive,
                        DateCreated = DateTimeOffset.Now,
                        DateModified = DateTimeOffset.Now
                    };
                    if (model.Data.Form.OtherPropertyIds?.Any() == true)
                    {
                        entry.RecipesOtherProperties = model.Data.Form.OtherPropertyIds.Distinct().Select(s =>
                            new RecipeOtherProperty
                            {
                                Recipe = entry,
                                OtherPropertyId = s,
                                DateCreated = DateTimeOffset.Now
                            }).ToList();
                    }

                    await _dbContext.Recipes.AddAsync(entry);
                    await _dbContext.SaveChangesAsync();

                    return RedirectToAction(nameof(Recipes));
                }
                else
                {
                    HttpContext.SetError($"Рецепт с названием {model.Data.Form.Name} уже существует.");
                }
            }

            model = await GetRecipeCreateViewModel(model.Data.Form);
            return View(model);
        }

        private async Task<ViewModel<RecipeUpdateViewModel>> GetRecipeUpdateViewModel(Recipe entry, RecipeUpdateViewModel.FormModel form = null)
        {
            var dishTypes = await _dbContext.DishTypes.Where(s => s.IsActive)
                .Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToListAsync();

            var cuisineTypes = await _dbContext.CuisineTypes.Where(s => s.IsActive)
                .Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToListAsync();

            var otherProperties = await _dbContext.OtherProperties.Where(s => s.IsActive)
                .Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToListAsync();

            return new ViewModel<RecipeUpdateViewModel>
            {
                Data = new RecipeUpdateViewModel
                {
                    DishTypes = dishTypes.OrderBy(s => s.Value).ToList(),
                    CuisineTypes = cuisineTypes.OrderBy(s => s.Value).ToList(),
                    OtherProperties = otherProperties.OrderBy(s => s.Value).ToList(),
                    Image = entry.Image,
                    Form = form ?? new RecipeUpdateViewModel.FormModel
                    {
                        Name = entry.Name,
                        Description = entry.Description,
                        PersonsCount = entry.PersonsCount,
                        IsActive = entry.IsActive,
                        DishTypeId = entry.DishTypeId,
                        CuisineTypeId = entry.CuisineTypeId,
                        OtherPropertyIds = entry.RecipesOtherProperties.Select(s => s.OtherPropertyId).ToList(),
                        RemoveImage = false
                    }
                }
            };
        }

        [HttpGet("Admin/Recipes/{id}")]
        public async Task<IActionResult> RecipeUpdate(int id)
        {
            var entry = await _dbContext.Recipes.AsNoTracking()
                .Include(s => s.RecipesOtherProperties)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (entry == null)
            {
                return NotFound();
            }
            var model = await GetRecipeUpdateViewModel(entry);
            return View(model);
        }

        [HttpPost("Admin/Recipes/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecipetUpdate(int id, ViewModel<RecipeUpdateViewModel> model)
        {
            if (model?.Data?.Form == null)
            {
                return BadRequest();
            }

            var entry = await _dbContext.Recipes.Include(s => s.RecipesOtherProperties)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (entry == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                model.Data.Form.Name = model.Data.Form.Name.ToLower();
                var entryWithSameName = await _dbContext.Recipes
                    .AnyAsync(s => s.Name == model.Data.Form.Name && s.Id != id);
                if (!entryWithSameName)
                {
                    string image = entry.Image;
                    if (model.Data.Form.Image != null)
                    {
                        if (model.Data.Form.Image.ContentType != "image/png"
                            && model.Data.Form.Image.ContentType != "image/jpeg")
                        {
                            HttpContext.SetError($"Разрешенный формат картинок: png, jpeg.");
                            model = await GetRecipeUpdateViewModel(entry, model.Data.Form);
                            return View(model);
                        }

                        if (!string.IsNullOrWhiteSpace(image))
                        {
                            System.IO.File.Delete(Path.Join(_environment.WebRootPath, image));
                        }

                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.Data.Form.Image.FileName)}";
                        image = $"/img/recipes/{fileName}";

                        using (var stream = System.IO.File.Create(Path.Join(_environment.WebRootPath, image)))
                        {
                            await model.Data.Form.Image.CopyToAsync(stream);
                        }

                    }
                    else if (model.Data.Form.RemoveImage && !string.IsNullOrWhiteSpace(image))
                    {
                        System.IO.File.Delete(Path.Join(_environment.WebRootPath, image));
                        image = null;
                    }

                    entry.Name = model.Data.Form.Name;
                    entry.Description = model.Data.Form.Description;
                    entry.Image = image;
                    entry.PersonsCount = model.Data.Form.PersonsCount;
                    entry.DishTypeId = model.Data.Form.DishTypeId;
                    entry.CuisineTypeId = model.Data.Form.CuisineTypeId;
                    entry.IsActive = model.Data.Form.IsActive;
                    entry.DateModified = DateTimeOffset.Now;

                    model.Data.Form.OtherPropertyIds = model.Data.Form.OtherPropertyIds?.Distinct().ToList() ?? new List<int>();

                    var otherPropertiesToAdd = model.Data.Form.OtherPropertyIds
                        .Where(s => !entry.RecipesOtherProperties.Any(x => x.OtherPropertyId == s))
                        .Select(s =>
                            new RecipeOtherProperty
                            {
                                Recipe = entry,
                                OtherPropertyId = s,
                                DateCreated = DateTimeOffset.Now
                            })
                        .ToList() ?? new List<RecipeOtherProperty>();

                    var otherPropertiesToRemove = entry.RecipesOtherProperties
                        .Where(s => !model.Data.Form.OtherPropertyIds.Any(x => x == s.OtherPropertyId))
                        .ToList();

                    await _dbContext.RecipeOtherProperties.AddRangeAsync(otherPropertiesToAdd);
                    _dbContext.RecipeOtherProperties.RemoveRange(otherPropertiesToRemove);

                    await _dbContext.SaveChangesAsync();

                    return RedirectToAction(nameof(Recipes));
                }
                else
                {
                    HttpContext.SetError($"Рецепт с названием {model.Data.Form.Name} уже существует.");
                }
            }

            model = await GetRecipeUpdateViewModel(entry, model.Data.Form);
            return View(model);
        }
        #endregion

        #region Ingredient
        [HttpGet]
        public async Task<IActionResult> Ingredients([FromQuery] int offset = 0, [FromQuery] int? limit = null,
            [FromQuery] string search = null, [FromQuery] bool showAll = false)
        {
            limit = limit ?? _appSettings.PageSize;
            var queryable = _dbContext.Ingredients.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                queryable = queryable.Where(s => s.Name.Contains(search));
            }
            if (!showAll)
            {
                queryable = queryable.Where(s => s.IsActive);
            }
            var total = await queryable.CountAsync();
            var entries = await queryable.OrderBy(s => s.Name).Skip(offset).Take(limit.Value).ToListAsync();
            return View(new ViewModel<IngredientsViewModel>
            {
                Data = new IngredientsViewModel
                {
                    Entries = entries
                },
                Paging = new Paging(total, offset, limit.Value, (o, l) => $"/Admin/Ingredients?offset={o}&limit={l}&search={search}&showAll={showAll}")
            });
        }

        [HttpGet("Admin/IngredientCreate")]
        public IActionResult IngredientCreate()
        {
            return View();
        }

        [HttpPost("Admin/IngredientCreate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IngredientCreate(ViewModel<IngredientCreateViewModel> model)
        {
            if (model?.Data?.Form == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                model.Data.Form.Name = model.Data.Form.Name.ToLower();
                var entryWithSameName = await _dbContext.Ingredients.AnyAsync(s => s.Name == model.Data.Form.Name);
                if (!entryWithSameName)
                {
                    string image = null;
                    if (model.Data.Form.Image != null)
                    {
                        if (model.Data.Form.Image.ContentType != "image/png"
                            && model.Data.Form.Image.ContentType != "image/jpeg")
                        {
                            HttpContext.SetError($"Разрешенный формат картинок: png, jpeg.");
                            return View(model);
                        }
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.Data.Form.Image.FileName)}";
                        image = $"/img/ingredients/{fileName}";

                        using (var stream = System.IO.File.Create(Path.Join(_environment.WebRootPath, image)))
                        {
                            await model.Data.Form.Image.CopyToAsync(stream);
                        }

                    }
                    var entry = new Ingredient
                    {
                        Name = model.Data.Form.Name,
                        Image = image,
                        IsActive = model.Data.Form.IsActive,
                        DateCreated = DateTimeOffset.Now,
                        DateModified = DateTimeOffset.Now
                    };
                    await _dbContext.Ingredients.AddAsync(entry);
                    await _dbContext.SaveChangesAsync();

                    return RedirectToAction(nameof(Ingredients));
                }
                else
                {
                    HttpContext.SetError($"Ингредиент с названием {model.Data.Form.Name} уже существует.");
                }
            }

            return View(model);
        }

        [HttpGet("Admin/Ingredients/{id}")]
        public async Task<IActionResult> IngredientUpdate(int id)
        {
            var entry = await _dbContext.Ingredients.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (entry == null)
            {
                return NotFound();
            }

            return View(new ViewModel<IngredientUpdateViewModel>
            {
                Data = new IngredientUpdateViewModel
                {
                    Image = entry.Image,
                    Form = new IngredientUpdateViewModel.FormModel
                    {
                        Name = entry.Name,
                        IsActive = entry.IsActive,
                        RemoveImage = false
                    }
                }
            });
        }

        [HttpPost("Admin/Ingredients/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IngredientUpdate(int id, ViewModel<IngredientUpdateViewModel> model)
        {
            if (model?.Data?.Form == null)
            {
                return BadRequest();
            }

            var entry = await _dbContext.Ingredients.FirstOrDefaultAsync(s => s.Id == id);
            if (entry == null)
            {
                return NotFound();
            }

            model.Data.Image = entry.Image;

            if (ModelState.IsValid)
            {
                model.Data.Form.Name = model.Data.Form.Name.ToLower();
                var entryWithSameName = await _dbContext.Ingredients
                    .AnyAsync(s => s.Name == model.Data.Form.Name && s.Id != id);
                if (!entryWithSameName)
                {
                    string image = entry.Image;
                    if (model.Data.Form.Image != null)
                    {
                        if (model.Data.Form.Image.ContentType != "image/png"
                            && model.Data.Form.Image.ContentType != "image/jpeg")
                        {
                            HttpContext.SetError($"Разрешенный формат картинок: png, jpeg.");
                            return View(model);
                        }

                        if (!string.IsNullOrWhiteSpace(image))
                        {
                            System.IO.File.Delete(Path.Join(_environment.WebRootPath, image));
                        }

                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.Data.Form.Image.FileName)}";
                        image = $"/img/ingredients/{fileName}";

                        using (var stream = System.IO.File.Create(Path.Join(_environment.WebRootPath, image)))
                        {
                            await model.Data.Form.Image.CopyToAsync(stream);
                        }

                    }
                    else if (model.Data.Form.RemoveImage && !string.IsNullOrWhiteSpace(image))
                    {
                        System.IO.File.Delete(Path.Join(_environment.WebRootPath, image));
                        image = null;
                    }

                    entry.Name = model.Data.Form.Name;
                    entry.Image = image;
                    entry.IsActive = model.Data.Form.IsActive;
                    entry.DateModified = DateTimeOffset.Now;

                    await _dbContext.SaveChangesAsync();

                    return RedirectToAction(nameof(Ingredients));
                }
                else
                {
                    HttpContext.SetError($"Ингредиент с названием {model.Data.Form.Name} уже существует.");
                }
            }

            return View(model);
        }
        #endregion

        #region CuisineType
        [HttpGet]
        public async Task<IActionResult> CuisineTypes([FromQuery] int offset = 0, [FromQuery] int? limit = null,
            [FromQuery] string search = null, [FromQuery] bool showAll = false)
        {
            limit = limit ?? _appSettings.PageSize;
            var queryable = _dbContext.CuisineTypes.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                queryable = queryable.Where(s => s.Name.Contains(search));
            }
            if (!showAll)
            {
                queryable = queryable.Where(s => s.IsActive);
            }
            var total = await queryable.CountAsync();
            var entries = await queryable.OrderBy(s => s.Name).Skip(offset).Take(limit.Value).ToListAsync();
            return View(new ViewModel<CuisineTypesViewModel>
            {
                Data = new CuisineTypesViewModel
                {
                    Entries = entries
                },
                Paging = new Paging(total, offset, limit.Value, (o, l) => $"/Admin/CuisineTypes?offset={o}&limit={l}&search={search}&showAll={showAll}")
            });
        }

        [HttpGet("Admin/CuisineTypeCreate")]
        public IActionResult CuisineTypeCreate()
        {
            return View();
        }

        [HttpPost("Admin/CuisineTypeCreate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CuisineTypeCreate(ViewModel<CuisineTypeCreateViewModel> model)
        {
            if (model?.Data?.Form == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                model.Data.Form.Name = model.Data.Form.Name.ToLower();
                var entryWithSameName = await _dbContext.CuisineTypes.AnyAsync(s => s.Name == model.Data.Form.Name);
                if (!entryWithSameName)
                {
                    var entry = new CuisineType
                    {
                        Name = model.Data.Form.Name,
                        IsActive = model.Data.Form.IsActive,
                        DateCreated = DateTimeOffset.Now,
                        DateModified = DateTimeOffset.Now
                    };
                    await _dbContext.CuisineTypes.AddAsync(entry);
                    await _dbContext.SaveChangesAsync();

                    return RedirectToAction(nameof(CuisineTypes));
                }
                else
                {
                    HttpContext.SetError($"Тип кухни с названием {model.Data.Form.Name} уже существует.");
                }
            }

            return View(model);
        }

        [HttpGet("Admin/CuisineTypes/{id}")]
        public async Task<IActionResult> CuisineTypeUpdate(int id)
        {
            var entry = await _dbContext.CuisineTypes.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (entry == null)
            {
                return NotFound();
            }

            return View(new ViewModel<CuisineTypeUpdateViewModel>
            {
                Data = new CuisineTypeUpdateViewModel
                {
                    Form = new CuisineTypeUpdateViewModel.FormModel
                    {
                        Name = entry.Name,
                        IsActive = entry.IsActive
                    }
                }
            });
        }

        [HttpPost("Admin/CuisineTypes/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CuisineTypeUpdate(int id, ViewModel<CuisineTypeUpdateViewModel> model)
        {
            if (model?.Data?.Form == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var entry = await _dbContext.CuisineTypes.FirstOrDefaultAsync(s => s.Id == id);
                if (entry == null)
                {
                    return NotFound();
                }
                else
                {
                    model.Data.Form.Name = model.Data.Form.Name.ToLower();
                    var entryWithSameName = await _dbContext.CuisineTypes
                        .AnyAsync(s => s.Name == model.Data.Form.Name && s.Id != id);
                    if (!entryWithSameName)
                    {
                        entry.Name = model.Data.Form.Name;
                        entry.IsActive = model.Data.Form.IsActive;
                        entry.DateModified = DateTimeOffset.Now;

                        await _dbContext.SaveChangesAsync();

                        return RedirectToAction(nameof(CuisineTypes));
                    }
                    else
                    {
                        HttpContext.SetError($"Тип кухни с названием {model.Data.Form.Name} уже существует.");
                    }
                }
            }

            return View(model);
        }
        #endregion

        #region DishType
        [HttpGet]
        public async Task<IActionResult> DishTypes([FromQuery] int offset = 0, [FromQuery] int? limit = null,
            [FromQuery] string search = null, [FromQuery] bool showAll = false)
        {
            limit = limit ?? _appSettings.PageSize;
            var queryable = _dbContext.DishTypes.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                queryable = queryable.Where(s => s.Name.Contains(search));
            }
            if (!showAll)
            {
                queryable = queryable.Where(s => s.IsActive);
            }
            var total = await queryable.CountAsync();
            var entries = await queryable.OrderBy(s => s.Name).Skip(offset).Take(limit.Value).ToListAsync();
            return View(new ViewModel<DishTypesViewModel>
            {
                Data = new DishTypesViewModel
                {
                    Entries = entries
                },
                Paging = new Paging(total, offset, limit.Value, (o, l) => $"/Admin/DishTypes?offset={o}&limit={l}&search={search}&showAll={showAll}")
            });
        }

        [HttpGet("Admin/DishTypeCreate")]
        public IActionResult DishTypeCreate()
        {
            return View();
        }

        [HttpPost("Admin/DishTypeCreate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DishTypeCreate(ViewModel<DishTypeCreateViewModel> model)
        {
            if (model?.Data?.Form == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                model.Data.Form.Name = model.Data.Form.Name.ToLower();
                var entryWithSameName = await _dbContext.DishTypes.AnyAsync(s => s.Name == model.Data.Form.Name);
                if (!entryWithSameName)
                {
                    var entry = new DishType
                    {
                        Name = model.Data.Form.Name,
                        IsActive = model.Data.Form.IsActive,
                        DateCreated = DateTimeOffset.Now,
                        DateModified = DateTimeOffset.Now
                    };
                    await _dbContext.DishTypes.AddAsync(entry);
                    await _dbContext.SaveChangesAsync();

                    return RedirectToAction(nameof(DishTypes));
                }
                else
                {
                    HttpContext.SetError($"Тип блюда с названием {model.Data.Form.Name} уже существует.");
                }
            }

            return View(model);
        }

        [HttpGet("Admin/DishTypes/{id}")]
        public async Task<IActionResult> DishTypeUpdate(int id)
        {
            var entry = await _dbContext.DishTypes.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (entry == null)
            {
                return NotFound();
            }

            return View(new ViewModel<DishTypeUpdateViewModel>
            {
                Data = new DishTypeUpdateViewModel
                {
                    Form = new DishTypeUpdateViewModel.FormModel
                    {
                        Name = entry.Name,
                        IsActive = entry.IsActive
                    }
                }
            });
        }

        [HttpPost("Admin/DishTypes/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DishTypeUpdate(int id, ViewModel<DishTypeUpdateViewModel> model)
        {
            if (model?.Data?.Form == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var entry = await _dbContext.DishTypes.FirstOrDefaultAsync(s => s.Id == id);
                if (entry == null)
                {
                    return NotFound();
                }
                else
                {
                    model.Data.Form.Name = model.Data.Form.Name.ToLower();
                    var entryWithSameName = await _dbContext.DishTypes
                        .AnyAsync(s => s.Name == model.Data.Form.Name && s.Id != id);
                    if (!entryWithSameName)
                    {
                        entry.Name = model.Data.Form.Name;
                        entry.IsActive = model.Data.Form.IsActive;
                        entry.DateModified = DateTimeOffset.Now;

                        await _dbContext.SaveChangesAsync();

                        return RedirectToAction(nameof(DishTypes));
                    }
                    else
                    {
                        HttpContext.SetError($"Тип блюда с названием {model.Data.Form.Name} уже существует.");
                    }
                }
            }

            return View(model);
        }
        #endregion

        #region QuantityType
        [HttpGet]
        public async Task<IActionResult> QuantityTypes([FromQuery] int offset = 0, [FromQuery] int? limit = null,
            [FromQuery] string search = null, [FromQuery] bool showAll = false)
        {
            limit = limit ?? _appSettings.PageSize;
            var queryable = _dbContext.QuantityTypes.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                queryable = queryable.Where(s => s.Name.Contains(search));
            }
            if (!showAll)
            {
                queryable = queryable.Where(s => s.IsActive);
            }
            var total = await queryable.CountAsync();
            var entries = await queryable.OrderBy(s => s.Name).Skip(offset).Take(limit.Value).ToListAsync();
            return View(new ViewModel<QuantityTypesViewModel>
            {
                Data = new QuantityTypesViewModel
                {
                    Entries = entries
                },
                Paging = new Paging(total, offset, limit.Value, (o, l) => $"/Admin/QuantityTypes?offset={o}&limit={l}&search={search}&showAll={showAll}")
            });
        }

        [HttpGet("Admin/QuantityTypeCreate")]
        public IActionResult QuantityTypeCreate()
        {
            return View();
        }

        [HttpPost("Admin/QuantityTypeCreate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuantityTypeCreate(ViewModel<QuantityTypeCreateViewModel> model)
        {
            if (model?.Data?.Form == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                model.Data.Form.Name = model.Data.Form.Name.ToLower();
                var entryWithSameName = await _dbContext.QuantityTypes.AnyAsync(s => s.Name == model.Data.Form.Name);
                if (!entryWithSameName)
                {
                    var entry = new QuantityType
                    {
                        Name = model.Data.Form.Name,
                        IsActive = model.Data.Form.IsActive,
                        DateCreated = DateTimeOffset.Now,
                        DateModified = DateTimeOffset.Now
                    };
                    await _dbContext.QuantityTypes.AddAsync(entry);
                    await _dbContext.SaveChangesAsync();

                    return RedirectToAction(nameof(QuantityTypes));
                }
                else
                {
                    HttpContext.SetError($"Единица измерения с названием {model.Data.Form.Name} уже существует.");
                }
            }

            return View(model);
        }

        [HttpGet("Admin/QuantityTypes/{id}")]
        public async Task<IActionResult> QuantityTypeUpdate(int id)
        {
            var entry = await _dbContext.QuantityTypes.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (entry == null)
            {
                return NotFound();
            }

            return View(new ViewModel<QuantityTypeUpdateViewModel>
            {
                Data = new QuantityTypeUpdateViewModel
                {
                    Form = new QuantityTypeUpdateViewModel.FormModel
                    {
                        Name = entry.Name,
                        IsActive = entry.IsActive
                    }
                }
            });
        }

        [HttpPost("Admin/QuantityTypes/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuantityTypeUpdate(int id, ViewModel<QuantityTypeUpdateViewModel> model)
        {
            if (model?.Data?.Form == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var entry = await _dbContext.QuantityTypes.FirstOrDefaultAsync(s => s.Id == id);
                if (entry == null)
                {
                    return NotFound();
                }
                else
                {
                    model.Data.Form.Name = model.Data.Form.Name.ToLower();
                    var entryWithSameName = await _dbContext.QuantityTypes
                        .AnyAsync(s => s.Name == model.Data.Form.Name && s.Id != id);
                    if (!entryWithSameName)
                    {
                        entry.Name = model.Data.Form.Name;
                        entry.IsActive = model.Data.Form.IsActive;
                        entry.DateModified = DateTimeOffset.Now;

                        await _dbContext.SaveChangesAsync();

                        return RedirectToAction(nameof(QuantityTypes));
                    }
                    else
                    {
                        HttpContext.SetError($"Единица измерения с названием {model.Data.Form.Name} уже существует.");
                    }
                }
            }

            return View(model);
        }
        #endregion


        #region OtherProperty
        [HttpGet]
        public async Task<IActionResult> OtherProperties([FromQuery] int offset = 0, [FromQuery] int? limit = null,
            [FromQuery] string search = null, [FromQuery] bool showAll = false)
        {
            limit = limit ?? _appSettings.PageSize;
            var queryable = _dbContext.OtherProperties.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                queryable = queryable.Where(s => s.Name.Contains(search));
            }
            if (!showAll)
            {
                queryable = queryable.Where(s => s.IsActive);
            }
            var total = await queryable.CountAsync();
            var entries = await queryable.OrderBy(s => s.Name).Skip(offset).Take(limit.Value).ToListAsync();
            return View(new ViewModel<OtherPropertiesViewModel>
            {
                Data = new OtherPropertiesViewModel
                {
                    Entries = entries
                },
                Paging = new Paging(total, offset, limit.Value, (o, l) => $"/Admin/OtherProperties?offset={o}&limit={l}&search={search}&showAll={showAll}")
            });
        }

        [HttpGet("Admin/OtherPropertyCreate")]
        public IActionResult OtherPropertyCreate()
        {
            return View();
        }

        [HttpPost("Admin/OtherPropertyCreate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OtherPropertyCreate(ViewModel<OtherPropertyCreateViewModel> model)
        {
            if (model?.Data?.Form == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                model.Data.Form.Name = model.Data.Form.Name.ToLower();
                var entryWithSameName = await _dbContext.OtherProperties.AnyAsync(s => s.Name == model.Data.Form.Name);
                if (!entryWithSameName)
                {
                    var entry = new OtherProperty
                    {
                        Name = model.Data.Form.Name,
                        IsActive = model.Data.Form.IsActive,
                        DateCreated = DateTimeOffset.Now,
                        DateModified = DateTimeOffset.Now
                    };
                    await _dbContext.OtherProperties.AddAsync(entry);
                    await _dbContext.SaveChangesAsync();

                    return RedirectToAction(nameof(OtherProperties));
                }
                else
                {
                    HttpContext.SetError($"Свойство с названием {model.Data.Form.Name} уже существует.");
                }
            }

            return View(model);
        }

        [HttpGet("Admin/OtherProperties/{id}")]
        public async Task<IActionResult> OtherPropertyUpdate(int id)
        {
            var entry = await _dbContext.OtherProperties.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (entry == null)
            {
                return NotFound();
            }

            return View(new ViewModel<OtherPropertyUpdateViewModel>
            {
                Data = new OtherPropertyUpdateViewModel
                {
                    Form = new OtherPropertyUpdateViewModel.FormModel
                    {
                        Name = entry.Name,
                        IsActive = entry.IsActive
                    }
                }
            });
        }

        [HttpPost("Admin/OtherProperties/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OtherPropertyUpdate(int id, ViewModel<OtherPropertyUpdateViewModel> model)
        {
            if (model?.Data?.Form == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var entry = await _dbContext.OtherProperties.FirstOrDefaultAsync(s => s.Id == id);
                if (entry == null)
                {
                    return NotFound();
                }
                else
                {
                    model.Data.Form.Name = model.Data.Form.Name.ToLower();
                    var entryWithSameName = await _dbContext.OtherProperties
                        .AnyAsync(s => s.Name == model.Data.Form.Name && s.Id != id);
                    if (!entryWithSameName)
                    {
                        entry.Name = model.Data.Form.Name;
                        entry.IsActive = model.Data.Form.IsActive;
                        entry.DateModified = DateTimeOffset.Now;

                        await _dbContext.SaveChangesAsync();

                        return RedirectToAction(nameof(OtherProperties));
                    }
                    else
                    {
                        HttpContext.SetError($"Свойство с названием {model.Data.Form.Name} уже существует.");
                    }
                }
            }

            return View(model);
        }
        #endregion


        #region User
        [HttpGet]
        public async Task<IActionResult> Users([FromQuery] int offset = 0, [FromQuery] int? limit = null,
            [FromQuery] string search = null, [FromQuery] bool showAll = false)
        {
            limit = limit ?? _appSettings.PageSize;
            var queryable = _dbContext.Users.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                queryable = queryable.Where(s => s.Email.Contains(search));
            }
            if (!showAll)
            {
                queryable = queryable.Where(s => s.IsActive);
            }
            var total = await queryable.CountAsync();
            var entries = await queryable.Skip(offset).Take(limit.Value).ToListAsync();
            return View(new ViewModel<UsersViewModel>
            {
                Data = new UsersViewModel
                {
                    Entries = entries
                },
                Paging = new Paging(total, offset, limit.Value, (o, l) => $"/Admin/Users?offset={o}&limit={l}&search={search}&showAll={showAll}")
            });
        }

        [HttpGet("Admin/Users/{id}")]
        public async Task<IActionResult> UserUpdate(int id)
        {
            var entry = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (entry == null)
            {
                return NotFound();
            }

            return View(new ViewModel<UserUpdateViewModel>
            {
                Data = new UserUpdateViewModel
                {
                    Email = entry.Email,
                    Name = entry.Name,
                    IsActive = entry.IsActive,
                    IsAdmin = entry.IsAdmin,
                    DateCreated = entry.DateCreated,
                    DateModified = entry.DateModified,
                    Form = new UserUpdateViewModel.FormModel
                    {
                        Id = entry.Id
                    }
                }
            });
        }

        [HttpPost("Admin/Users/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserUpdate(int id, ViewModel<UserUpdateViewModel> model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var entry = await _dbContext.Users.FirstOrDefaultAsync(s => s.Id == id);
            if (entry == null)
            {
                return NotFound();
            }

            if (!entry.IsAdmin)
            {
                entry.IsActive = !entry.IsActive;
                entry.DateModified = DateTimeOffset.Now;
                await _dbContext.SaveChangesAsync();
            }

            return View(new ViewModel<UserUpdateViewModel>
            {
                Data = new UserUpdateViewModel
                {
                    Email = entry.Email,
                    Name = entry.Name,
                    IsActive = entry.IsActive,
                    IsAdmin = entry.IsAdmin,
                    DateCreated = entry.DateCreated,
                    DateModified = entry.DateModified,
                    Form = new UserUpdateViewModel.FormModel
                    {
                        Id = entry.Id
                    }
                }
            });
        }
        #endregion

    }
}