using System.Runtime.CompilerServices;
using API.DTO;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(i => i.Id == id);
            return user;
        }

        public async Task<AppUser> GetUserByUserNameAsync(string name)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(i => i.UserName == name);
            return user;
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            var user = await _context.Users.Include(p => p.Photos).ToListAsync();
            return user;
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified; 
        }

        public async Task<MemberDTO> GetMemberAsync(string username)
        {
            return await _context.Users.Where(x => x.UserName == username)
            .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
        }

        public async Task<PageList<MemberDTO>> GetMembersAsync(UserParams userParams)
        {
            // var query = _context.Users.ProjectTo<MemberDTO>(_mapper.ConfigurationProvider).AsNoTracking();
            var query = _context.Users.AsQueryable();
            query = query.Where(u => u.UserName != userParams.CurrentUserName);
            query = query.Where(g => g.Gender == userParams.Gender);

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1).Date;
            var maxDob =  DateTime.Today.AddYears(-userParams.MinAge - 1).Date;
            // var minDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            // var maxDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob); 
            query =userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _=> query.OrderByDescending(u => u.LastActive)     
            };
            return await PageList<MemberDTO>.CreateAsync(query.AsNoTracking().ProjectTo<MemberDTO>(_mapper.ConfigurationProvider),
             userParams.PageNumber, userParams.PageSize);
        }
    }
}