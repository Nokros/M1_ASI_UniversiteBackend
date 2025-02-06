using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.UeUseCases.Create;

namespace UniversiteDomainUnitTests;

public class UeUnitTest
{
    [SetUp]
    public void Setup()
    {
    }
    [Test]
    public async Task CreateEtudiantUseCase()
    {
        long id = 1;
        string NumUe = "INFO-01";
        string intituleUe = "Informatique";
        
        // On crée l'étudiant qui doit être ajouté en base
        Ue ueSansID = new Ue{Intitule = intituleUe, NumeroUe = NumUe};
        //  Créons le mock du repository
        // On initialise une fausse datasource qui va simuler une UeRepository
        var mock = new Mock<IUeRepository>();
        
        // Il faut ensuite aller dans le use case pour voir quelles fonctions simuler
        // Nous devons simuler FindByCondition et Create
        
        // Simulation de la fonction FindByCondition
        // On dit à ce mock que l'ue n'existe pas déjà
        // La réponse à l'appel FindByCondition est donc une liste vide
        var reponseFindByCondition = new List<Ue>();
        // On crée un bouchon dans le mock pour la fonction FindByCondition
        // Quelque soit le paramètre de la fonction FindByCondition, on renvoie la liste vide
        mock.Setup(repo=>repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>())).ReturnsAsync(reponseFindByCondition);
        
        // Simulation de la fonction Create
        // On lui dit que l'ajout d'une Ue renvoie une Ue avec l'Id 1
        Ue ueCree = new Ue{Id= id,Intitule = intituleUe, NumeroUe = NumUe};
        mock.Setup(repoUe=>repoUe.CreateAsync(ueSansID)).ReturnsAsync(ueCree);
        
        // On crée le bouchon (un faux etudiantRepository). Il est prêt à être utilisé
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto=>facto.UeRepository()).Returns(mock.Object);
        
        // Création du use case en injectant notre faux repository
        CreateUeUseCase useCase=new CreateUeUseCase(mockFactory.Object);
        // Appel du use case
        var ueTeste = await useCase.ExecuteAsync(ueSansID);
        
        // Vérification du résultat
        Assert.That(ueTeste.Id, Is.EqualTo(ueCree.Id));
        Assert.That(ueTeste.Intitule, Is.EqualTo(ueCree.Intitule));
        Assert.That(ueTeste.NumeroUe, Is.EqualTo(ueCree.NumeroUe));
    }
}