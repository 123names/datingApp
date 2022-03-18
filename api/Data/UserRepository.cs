using AutoMapper.QueryableExtensions;

namespace api.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            this.mapper = mapper;
            this.context = context;
        }

        public async Task<MemberDto> GetMemberByUsernameAsync(string username, bool isCurrentUser)
        {
            var query = this.context.Users
                .Where(user => user.UserName == username)
                .ProjectTo<MemberDto>(this.mapper.ConfigurationProvider)
                .AsQueryable();

            if (isCurrentUser) query = query.IgnoreQueryFilters();

            return await query.FirstOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = this.context.Users.AsQueryable();

            // add filter to query to return filtered result
            query = query.Where(user => user.UserName != userParams.CurrentUserName);
            query = query.Where(user => user.Gender == userParams.Gender);

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
            query = query.Where(user => user.DateOfBirth >= minDob && user.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(user => user.UserCreatedOn),
                "age" => query.OrderByDescending(user => user.DateOfBirth),
                _ => query.OrderByDescending(user => user.LastActive),
            };
            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(this.mapper.ConfigurationProvider).AsNoTracking(),
            userParams.PageNumber, userParams.PageSize);
        }
        public void Update(AppUser user)
        {
            this.context.Entry(user).State = EntityState.Modified;
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await this.context.Users.FindAsync(id);
        }
        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await this.context.Users
                .Include(p => p.UserPhotos)
                .SingleOrDefaultAsync(user => user.UserName == username);
        }
        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await this.context.Users
                .Include(p => p.UserPhotos)
                .ToListAsync();
        }

        public async Task<string> GetUserGender(string username)
        {
            return await this.context.Users
                .Where(x => x.UserName == username)
                .Select(x => x.Gender)
                .FirstOrDefaultAsync();
        }

        public async Task<AppUser> GetUserByPhotoId(int photoId)
        {
            return await this.context.Users
                .Include(p => p.UserPhotos)
                .IgnoreQueryFilters()
                .Where(p => p.UserPhotos.Any(p => p.Id == photoId))
                .FirstOrDefaultAsync();
        }
    }
}