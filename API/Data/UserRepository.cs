using API.DTO;
using API.Entities;
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

        public async Task<IEnumerable<MemberDTO>> GetMembersAsync()
        {
            return await _context.Users.ProjectTo<MemberDTO>(_mapper.ConfigurationProvider).ToListAsync();
        }
    }
}