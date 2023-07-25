var imgFeature = document.querySelector('.loaded');
var listImgProduct = document.querySelectorAll('#swiper-wrapper-1ce3af5f7e952345 img');
var listImgThumbs = document.querySelectorAll('#swiper-wrapper-251c721a441062eab img');
var prevBtn = document.querySelector('.carousel-control-prev');
var nextBtn = document.querySelector('.carousel-control-next');

var currentIndex = 0;
var isClickEventActive = false;
var counter = 0;

function updateImageByIndex(index) {

    document.querySelectorAll('#swiper-wrapper-251c721a441062eab div').forEach(item => {
        item.classList.remove('swiper-slide-thumb-active');
    });

    currentIndex = index;
    imgFeature.setAttribute('src', listImgProduct[index].getAttribute('data-src'));
    listImgThumbs[index].parentElement.parentElement.classList.add('swiper-slide-thumb-active');
}
listImgThumbs.forEach((thumb, index) => {
    thumb.addEventListener('click', (e) => {
        if (currentIndex < index) {
            imgFeature.style.animation = '';
            setTimeout(() => {
                updateImageByIndex(index);
                imgFeature.style.animation = 'slideLeft 0.5s ease-in-out forwards';
            }, 200);
        } else if (currentIndex > index) {
            imgFeature.style.animation = '';
            setTimeout(() => {
                updateImageByIndex(index);
                imgFeature.style.animation = 'slideRight 0.5s ease-in-out forwards';
            }, 200);
        }
    });
});

prevBtn.addEventListener('click', (e) => {

    if (!isClickEventActive) {
        isClickEventActive = true;
        if (currentIndex == 0) {
            currentIndex = listImgThumbs.length - 1;
        } else {
            currentIndex--;
        }
        imgFeature.style.animation = '';

        setTimeout(() => {
            updateImageByIndex(currentIndex);
            imgFeature.style.animation = 'slideRight 0.5s ease-in-out forwards';
        }, 200);

        setTimeout(() => {
            isClickEventActive = false;
        }, 100);

    }
});

nextBtn.addEventListener('click', (e) => {

    if (!isClickEventActive) {
        isClickEventActive = true;
        if (currentIndex == listImgThumbs.length - 1) {
            currentIndex = 0;
        } else {
            currentIndex++;
        }
        imgFeature.style.animation = '';

        setTimeout(() => {
            updateImageByIndex(currentIndex);
            imgFeature.style.animation = 'slideLeft 0.5s ease-in-out forwards';
        }, 100);

        setTimeout(() => {
            isClickEventActive = false;
        }, 100);
    }  

});
//function slideAllImages() {
//    let slideInterval = setInterval(() => {
//        if (!isClickEventActive) {
//            let nextIndex = (currentIndex + 1) % listImgProduct.length;
//            imgFeature.style.animation = '';
//            setTimeout(() => {
//                updateImageByIndex(nextIndex);
//                imgFeature.style.animation = 'slideLeft 0.5s ease-in-out forwards';
//            }, 100);
//        }
//    }, 5000);
//};

//slideAllImages();