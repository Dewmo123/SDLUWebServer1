using Repositories;

namespace BusinessLayer.Services
{
    public abstract class Service
    {
        protected string _dbAddress;
        protected RepositoryManager _repositoryManager;
        public Service(RepositoryManager repo,string dbAddress)
        {
            _dbAddress = dbAddress;
            _repositoryManager = repo;
        }
    }
}
