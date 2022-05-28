using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyCuisine.Web.Data;
using MyCuisine.Web.Models;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Web;

namespace MyCuisine.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationContext _dbContext;
        private readonly ILogger<HomeController> _logger;
        private readonly AppSettings _appSettings;

        public HomeController(ApplicationContext dbContext, ILogger<HomeController> logger, AppSettings appSettings)
        {
            _dbContext = dbContext;
            _logger = logger;
            _appSettings = appSettings;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index([FromQuery] int offset = 0, [FromQuery] int? limit = null,
            [FromQuery] FilterViewModel filter = null)
        {
            limit = limit ?? _appSettings.PageSize;
            var queryable = _dbContext.Recipes.AsNoTracking()
                .Where(s => s.IsActive)
                .AsQueryable();

            queryable = ApplyFilter(queryable, filter?.Form);

            var total = await queryable.CountAsync();

            queryable = ApplySorting(queryable, filter?.Form);

            var userId = User.Identity.IsAuthenticated == true
                ? int.Parse(User.Claims.First(s => s.Type == ClaimTypes.Sid).Value)
                : 0;
            var entries = await queryable
                .Skip(offset).Take(limit.Value)
                .Select(s => new SummaryRecipesViewModel.SummaryRecipeViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Image = s.Image,
                    Rate = s.Rate,
                    Votes = s.Votes,
                    CuisineTypeId = s.CuisineTypeId,
                    CuisineType = s.CuisineType.Name,
                    DishTypeId = s.DishTypeId,
                    DishType = s.DishType.Name,
                    OtherProperties = s.RecipesOtherProperties
                        .Where(x => x.OtherProperty.IsActive)
                        .Select(x => new OptionViewModel
                        {
                            Id = x.OtherProperty.Id,
                            Name = x.OtherProperty.Name
                        })
                        .ToList(),
                    DateModified = s.DateModified,
                    IsSaved = userId > 0
                        ? s.UserRecipes.Any(x => x.UserId == userId)
                        : false
                })
                .ToListAsync();

            var queryString = HttpUtility.ParseQueryString(HttpContext.Request.QueryString.ToString());
            queryString.Remove("offset");
            queryString.Remove("limit");

            return View(new ViewModel<SummaryRecipesViewModel>
            {
                Data = new SummaryRecipesViewModel
                {
                    Filter = await GetFilter(filter?.Form),
                    Entries = entries
                },
                Paging = new Paging(total, offset, limit.Value, (o, l) =>
                    $"/{(queryString.Count > 0 ? $"?{queryString}&offset={o}&limit={l}" : $"?offset={o}&limit={l}")}")
            });
        }

        [HttpGet("RecipeDetail/Info/{recipeId}")]
        public async Task<IActionResult> RecipeDetailInfo(int recipeId)
        {
            var entry = await _dbContext.Recipes
                .Select(s => new RecipeDetailInfoViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Image = s.Image,
                    Rate = s.Rate,
                    Votes = s.Votes,
                    CuisineType = s.CuisineType.Name,
                    DishType = s.DishType.Name,
                    OtherProperties = s.RecipesOtherProperties.Where(s => s.OtherProperty.IsActive)
                        .Select(x => x.OtherProperty.Name).ToList()
                })
                .FirstOrDefaultAsync(s => s.Id == recipeId);

            if (entry == null)
            {
                return NotFound();
            }

            return View(new ViewModel<RecipeDetailInfoViewModel>
            {
                Data = entry
            });
        }

        [HttpGet("RecipeDetail/Ingredients/{recipeId}")]
        public async Task<IActionResult> RecipeDetailIngredients(int recipeId)
        {
            var entry = await _dbContext.Recipes
                .Select(s => new RecipeDetailIngredientsViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Ingredients = s.RecipeItems.Select(x => new RecipeDetailIngredientsViewModel.IngredientViewModel
                    {
                        Name = x.Ingredient.Name,
                        Image = x.Ingredient.Image,
                        IsMain = x.IsMain,
                        OrderNumber = x.OrderNumber,
                        Quantity = x.Quantity,
                        QuantityType = x.QuantityType.Name
                    }).ToList()
                })
                .FirstOrDefaultAsync(s => s.Id == recipeId);

            if (entry == null)
            {
                return NotFound();
            }

            return View(new ViewModel<RecipeDetailIngredientsViewModel>
            {
                Data = entry
            });
        }

        [HttpGet("RecipeDetail/CookingSteps/{recipeId}")]
        public async Task<IActionResult> RecipeDetailCookingSteps(int recipeId)
        {
            var entry = await _dbContext.Recipes
                .Select(s => new RecipeDetailCookingStepsViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    CookingSteps = s.CookingSteps.Select(x => new RecipeDetailCookingStepsViewModel.CookingStepViewModel
                    {
                        Name = x.Name,
                        Description = x.Description,
                        Image = x.Image,
                        OrderNumber= x.OrderNumber
                    }).ToList()
                })
                .FirstOrDefaultAsync(s => s.Id == recipeId);

            if (entry == null)
            {
                return NotFound();
            }

            return View(new ViewModel<RecipeDetailCookingStepsViewModel>
            {
                Data = entry
            });
        }

        [HttpGet("RecipeDetail/Rates/{recipeId}")]
        public async Task<IActionResult> RecipeDetailRates(int recipeId, [FromQuery] int offset = 0, [FromQuery] int? limit = null)
        {
            limit = limit ?? _appSettings.PageSize;
            var entry = await _dbContext.Recipes
                .Select(s => new RecipeDetailRatesViewModel
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .FirstOrDefaultAsync(s => s.Id == recipeId);

            if (entry == null)
            {
                return NotFound();
            }

            var total = await _dbContext.RecipeRates.CountAsync(s => s.RecipeId == recipeId);

            entry.Rates = await _dbContext.RecipeRates
                .Where(s => s.RecipeId == recipeId)
                .Select(s => new RecipeDetailRatesViewModel.RateViewModel
                {
                    User = s.User.Name,
                    Comment = s.Comment,
                    Rate = s.Rate.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    DateCreated = s.DateCreated,
                    DateModified = s.DateModified
                })
                .OrderByDescending(s => s.DateModified)
                .Skip(offset).Take(limit.Value)
                .ToListAsync();

            return View(new ViewModel<RecipeDetailRatesViewModel>
            {
                Data = entry,
                Paging = new Paging(total, offset, limit.Value, (o, l) => $"/RecipeDetail/Rates/{recipeId}?offset={o}&limit={l}")
            });
        }

        [Authorize]
        [HttpGet("RecipeDetail/RatesAdd/{recipeId}")]
        public async Task<IActionResult> RecipeDetailRatesAdd(int recipeId)
        {
            var userId = int.Parse(User.Claims.First(s => s.Type == ClaimTypes.Sid).Value);

            var recipe = await _dbContext.Recipes
                .Where(s => s.Id == recipeId)
                .Select(s => new RecipeDetailRatesAddViewModel
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .FirstOrDefaultAsync();

            if (recipe == null)
            {
                return NotFound();
            }

            var rate = await _dbContext.RecipeRates.AsNoTracking()
                .FirstOrDefaultAsync(s => s.RecipeId == recipeId && s.UserId == userId);

            if (rate != null)
            {
                recipe.Form = new RecipeDetailRatesAddViewModel.FormModel
                {
                    Rate = rate.Rate.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    Comment = rate.Comment
                };
            }

            return View(new ViewModel<RecipeDetailRatesAddViewModel>
            {
                Data = recipe
            });
        }

        [Authorize]
        [HttpPost("RecipeDetail/RatesAdd/{recipeId}")]
        public async Task<IActionResult> RecipeDetailRatesAdd(int recipeId, ViewModel<RecipeDetailRatesAddViewModel> model)
        {
            if (model?.Data?.Form == null)
            {
                return BadRequest();
            }

            var recipe = await _dbContext.Recipes
                .FirstOrDefaultAsync(s => s.Id == recipeId);

            if (recipe == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.Claims.First(s => s.Type == ClaimTypes.Sid).Value);
                var entry = await _dbContext.RecipeRates
                    .FirstOrDefaultAsync(s => s.RecipeId == recipeId && s.UserId == userId);

                bool isDelete = string.IsNullOrWhiteSpace(model.Data.Form.Rate) && string.IsNullOrWhiteSpace(model.Data.Form.Comment);
                var now = DateTimeOffset.Now;
                float localRate = 0;
                if (!string.IsNullOrWhiteSpace(model.Data.Form.Rate))
                {
                    localRate = float.Parse(model.Data.Form.Rate, System.Globalization.CultureInfo.InvariantCulture);
                }
                
                using (var transaction = await _dbContext.Database.BeginTransactionAsync())
                {
                    if (entry != null)
                    {
                        if (isDelete)
                        {
                            _dbContext.RecipeRates.Remove(entry);
                        }
                        else
                        {
                            entry.Rate = localRate;
                            entry.Comment = model.Data.Form.Comment;
                            entry.DateModified = now;
                        }
                    }
                    else if (!isDelete)
                    {
                        await _dbContext.RecipeRates.AddAsync(new MyCuisine.Data.Web.Models.RecipeRate
                        {
                            Rate = localRate,
                            Comment = model.Data.Form.Comment,
                            RecipeId = recipeId,
                            UserId = userId,
                            DateCreated = now,
                            DateModified = now
                        });
                    }
                    await _dbContext.SaveChangesAsync();
                    SqlParameter param = new SqlParameter("@recipeId", recipeId);
                    await _dbContext.Database.ExecuteSqlRawAsync($@"
UPDATE r
SET r.[Rate] = ISNULL(stat.[Rate], 0),
	r.[Votes] = ISNULL(stat.[Votes], 0)
FROM [Recipes] r
OUTER APPLY (
	SELECT
	AVG(rr.[Rate]) AS [Rate],
	COUNT(*) AS [Votes]
	FROM [RecipeRates] rr
	WHERE rr.[RecipeId] = @recipeId
) stat
WHERE r.[Id] = @recipeId
                    ", param);

                    await transaction.CommitAsync();
                }

                return LocalRedirect($"/RecipeDetail/Rates/{recipeId}");
            }

            var newModel = new ViewModel<RecipeDetailRatesAddViewModel>
            {
                Data = new RecipeDetailRatesAddViewModel
                {
                    Id = recipe.Id,
                    Name = recipe.Name,
                    Form = model.Data.Form
                }
            };
            return View(newModel);
        }

        [Authorize]
        [HttpGet("Bookmark")]
        public async Task<IActionResult> Bookmark([FromQuery] int offset = 0, [FromQuery] int? limit = null,
            [FromQuery] FilterViewModel filter = null)
        {
            limit = limit ?? _appSettings.PageSize;
            var userId = int.Parse(User.Claims.First(s => s.Type == ClaimTypes.Sid).Value);
            var queryable = _dbContext.UserRecipes.AsNoTracking()
                .Where(s => s.UserId == userId && s.Recipe.IsActive)
                .AsQueryable();

            queryable = ApplyFilter(queryable, filter?.Form);

            var total = await queryable.CountAsync();

            queryable = ApplySorting(queryable, filter?.Form);

            var entries = await queryable
                .Skip(offset).Take(limit.Value)
                .Select(s => new SummaryRecipesViewModel.SummaryRecipeViewModel
                {
                    Id = s.Recipe.Id,
                    Name = s.Recipe.Name,
                    Description = s.Recipe.Description,
                    Image = s.Recipe.Image,
                    Rate = s.Recipe.Rate,
                    Votes = s.Recipe.Votes,
                    CuisineTypeId = s.Recipe.CuisineTypeId,
                    CuisineType = s.Recipe.CuisineType.Name,
                    DishTypeId = s.Recipe.DishTypeId,
                    DishType = s.Recipe.DishType.Name,
                    OtherProperties = s.Recipe.RecipesOtherProperties
                        .Where(x => x.OtherProperty.IsActive)
                        .Select(x => new OptionViewModel
                        {
                            Id = x.OtherProperty.Id,
                            Name = x.OtherProperty.Name
                        })
                        .ToList(),
                    DateModified = s.Recipe.DateModified,
                    IsSaved = true
                })
                .ToListAsync();

            var queryString = HttpUtility.ParseQueryString(HttpContext.Request.QueryString.ToString());
            queryString.Remove("offset");
            queryString.Remove("limit");

            return View(new ViewModel<SummaryRecipesViewModel>
            {
                Data = new SummaryRecipesViewModel
                {
                    Filter = await GetFilter(filter?.Form),
                    Entries = entries
                },
                Paging = new Paging(total, offset, limit.Value, (o, l) =>
                    $"/Bookmark{(queryString.Count > 0 ? $"?{queryString}&offset={o}&limit={l}" : $"?offset={o}&limit={l}")}")
            });
        }

        [Authorize]
        [HttpPost("Bookmark/{recipeId}")]
        public async Task<IActionResult> Bookmark(int recipeId)
        {
            var userId = int.Parse(User.Claims.First(s => s.Type == ClaimTypes.Sid).Value);
            var userRecipe = await _dbContext.UserRecipes
                .FirstOrDefaultAsync(x => x.UserId == userId && x.RecipeId == recipeId);

            if (userRecipe == null)
            {
                await _dbContext.UserRecipes.AddAsync(new MyCuisine.Data.Web.Models.UserRecipe
                {
                    UserId = userId,
                    RecipeId = recipeId,
                    DateCreated = DateTimeOffset.Now
                });
            }
            else
            {
                _dbContext.UserRecipes.Remove(userRecipe);
            }

            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<FilterViewModel> GetFilter(FilterViewModel.FormModel filterForm = null)
        {
            var dishTypes = await _dbContext.DishTypes.Where(s => s.IsActive)
                .Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToListAsync();

            var cuisineTypes = await _dbContext.CuisineTypes.Where(s => s.IsActive)
                .Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToListAsync();

            var otherProperties = await _dbContext.OtherProperties.Where(s => s.IsActive)
                .Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToListAsync();

            return new FilterViewModel
            {
                DishTypes = dishTypes.OrderBy(s => s.Value).ToList(),
                CuisineTypes = cuisineTypes.OrderBy(s => s.Value).ToList(),
                OtherProperties = otherProperties.OrderBy(s => s.Value).ToList(),
                Form = filterForm
            };
        }

        private IQueryable<MyCuisine.Data.Web.Models.Recipe> ApplyFilter(
            IQueryable<MyCuisine.Data.Web.Models.Recipe> queryable, FilterViewModel.FormModel filterForm)
        {
            if (filterForm != null)
            {
                if (filterForm.DishTypeIds?.Any() == true)
                {
                    queryable = queryable.Where(s => filterForm.DishTypeIds.Contains(s.DishTypeId));
                }
                if (filterForm.CuisineTypeIds?.Any() == true)
                {
                    queryable = queryable.Where(s => filterForm.CuisineTypeIds.Contains(s.CuisineTypeId));
                }
                if (filterForm.OtherPropertyIds?.Any() == true)
                {
                    queryable = queryable.Where(s => s.RecipesOtherProperties.Any(x => filterForm.OtherPropertyIds.Contains(x.OtherPropertyId)));
                }
            }
            return queryable;
        }

        private IQueryable<MyCuisine.Data.Web.Models.UserRecipe> ApplyFilter(
            IQueryable<MyCuisine.Data.Web.Models.UserRecipe> queryable, FilterViewModel.FormModel filterForm)
        {
            if (filterForm != null)
            {
                if (filterForm.DishTypeIds?.Any() == true)
                {
                    queryable = queryable.Where(s => filterForm.DishTypeIds.Contains(s.Recipe.DishTypeId));
                }
                if (filterForm.CuisineTypeIds?.Any() == true)
                {
                    queryable = queryable.Where(s => filterForm.CuisineTypeIds.Contains(s.Recipe.CuisineTypeId));
                }
                if (filterForm.OtherPropertyIds?.Any() == true)
                {
                    queryable = queryable.Where(s => s.Recipe.RecipesOtherProperties.Any(x => filterForm.OtherPropertyIds.Contains(x.OtherPropertyId)));
                }
            }
            return queryable;
        }

        private IQueryable<MyCuisine.Data.Web.Models.Recipe> ApplySorting(
            IQueryable<MyCuisine.Data.Web.Models.Recipe> queryable, FilterViewModel.FormModel filterForm)
        {
            if (filterForm != null)
            {
                if (filterForm.SortBy == SortingOption.Rate)
                {
                    queryable = queryable.OrderByDescending(s => s.Rate);
                }
                else if (filterForm.SortBy == SortingOption.Vote)
                {
                    queryable = queryable.OrderByDescending(s => s.Votes);
                }
                else if (filterForm.SortBy == SortingOption.Alphabetical)
                {
                    queryable = queryable.OrderBy(s => s.Name);
                }
                else
                {
                    queryable = queryable.OrderByDescending(s => s.DateModified);
                }
            }
            else
            {
                queryable = queryable.OrderByDescending(s => s.DateModified);
            }

            return queryable;
        }

        private IQueryable<MyCuisine.Data.Web.Models.UserRecipe> ApplySorting(
            IQueryable<MyCuisine.Data.Web.Models.UserRecipe> queryable, FilterViewModel.FormModel filterForm)
        {
            if (filterForm != null)
            {
                if (filterForm.SortBy == SortingOption.Rate)
                {
                    queryable = queryable.OrderByDescending(s => s.Recipe.Rate);
                }
                else if (filterForm.SortBy == SortingOption.Vote)
                {
                    queryable = queryable.OrderByDescending(s => s.Recipe.Votes);
                }
                else if (filterForm.SortBy == SortingOption.Alphabetical)
                {
                    queryable = queryable.OrderBy(s => s.Recipe.Name);
                }
                else
                {
                    queryable = queryable.OrderByDescending(s => s.Recipe.DateModified);
                }
            }
            else
            {
                queryable = queryable.OrderByDescending(s => s.Recipe.DateModified);
            }

            return queryable;
        }
    }
}