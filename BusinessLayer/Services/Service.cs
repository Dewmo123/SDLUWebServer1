using AutoMapper;
using Repositories;

namespace BusinessLayer.Services
{
    public abstract class Service
    {
        protected string _dbAddress;
        protected RepositoryManager _repositoryManager;
        protected IMapper _mapper;
        public Service(RepositoryManager repo,IMapper mapper,string dbAddress)
        {
            _mapper = mapper;
            _dbAddress = dbAddress;
            _repositoryManager = repo;
        }
    }
}
