// Sample news data (you will replace this with your actual data)
const newsData = [
    {
        title: "News Title 1",
        image: "news-image-1.jpg",
        date: new Date("2023-09-01"),
        shortText: "Short text for news 1...",
        fullText: "Full text for news 1..."
    },
    {
        title: "News Title 2",
        image: "news-image-2.jpg",
        date: new Date("2023-09-02"),
        shortText: "Short text for news 2...",
        fullText: "Full text for news 2..."
    },
    // Add more news objects here
];

const newsCardsContainer = document.getElementById('news-cards');
const loadMoreButton = document.getElementById('load-more');

function createNewsCard(news) {
    // Format the date as "Month Day, Year"
    const formattedDate = news.date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    });

    const cardHtml = `
        <div class="col-md-4 mb-4">
            <div class="card">
                <img src="${news.image}" class="card-img-top" alt="${news.title}">
                <div class="card-body">
                    <h5 class="card-title">${news.title}</h5>
                    <p class="card-text">${news.shortText}</p>
                    <p class="card-date">${formattedDate}</p>
                    <a href="#" class="btn btn-primary">Learn More</a>
                </div>
            </div>
        </div>
    `;
    return cardHtml;
}

function loadNews() {
    const newsToLoad = 3; // Number of news cards to load at a time
    for (let i = 0; i < newsToLoad; i++) {
        if (newsData.length > 0) {
            const news = newsData.shift();
            const cardHtml = createNewsCard(news);
            newsCardsContainer.innerHTML += cardHtml;
        } else {
            loadMoreButton.style.display = 'none'; // Hide the "Load More" button when there's no more news
            break;
        }
    }
}

loadNews();

loadMoreButton.addEventListener('click', loadNews);
