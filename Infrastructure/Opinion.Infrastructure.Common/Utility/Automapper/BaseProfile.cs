
namespace Opinion.Infrastructure.Common.Utility
{
    using AutoMapper;
    public abstract class BaseProfile : Profile
    {
        private readonly string _profileName;
        protected BaseProfile(string profileName)
        {
            _profileName = profileName;
        }

        public override string ProfileName
        {
            get { return _profileName; }
        }

        protected override void Configure()
        {
            //TODO: Revisar los formateadores
            //ForSourceType<DateTime>().AddFormatter<StandardDateTimeFormatter>();
            //ForSourceType<DateTime?>().AddFormatter<StandardDateTimeFormatter>();            
            CreateMaps();
        }

        protected abstract void CreateMaps();
    }
}