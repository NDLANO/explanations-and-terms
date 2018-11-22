using ConceptsMicroservice.Models;
using ConceptsMicroservice.Repositories;
using ConceptsMicroservice.Services.Validation;
using FakeItEasy;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestServices.Validation
{
    public class ConceptValidationServiceTest
    {
        private readonly IConceptValidationService _validationService;
        private readonly IStatusRepository _statusRepository;

        public ConceptValidationServiceTest()
        {
            _statusRepository = A.Fake<IStatusRepository>();
            _validationService = new ConceptValidationService(_statusRepository);
        }
        #region StatusIdIsValid
        [Fact]
        public void StatusIdIsValidId_Returns_False_When_Id_Does_Not_Exist_In_The_DB()
        {
            A.CallTo(() => _statusRepository.GetById(A<int>._)).Returns(null);

            Assert.False(_validationService.StatusIdIsValidId(0));
        }
        [Fact]
        public void StatusIdIsValidId_Returns_True_When_Id_Does_Exist_In_The_DB()
        {
            A.CallTo(() => _statusRepository.GetById(A<int>._)).Returns(new Status());

            Assert.True(_validationService.StatusIdIsValidId(0));
        }
        #endregion
    }
}
