var nextlv1 = document.querySelector('.fa-angle-down')
var nextlv2 = document.querySelector('.dropdown-menu .nav-item > .fa-angle-down');

isClickEventActive = false;

console.log(nextlv1);
console.log(nextlv2);

nextlv1.addEventListener('click', (e) => {
    if (!isClickEventActive) {
        isClickEventActive = true;
        nextlv1.classList.add('fa-minus');
        nextlv1.parentElement.classList.add('active');
    } else {
        isClickEventActive = false;
        nextlv1.classList.remove('fa-minus');
        nextlv1.parentElement.classList.remove('active');
    }
});
nextlv2.addEventListener('click', (e) => {
    if (!isClickEventActive) {
        isClickEventActive = true;
        nextlv2.classList.add('fa-minus');
        nextlv2.parentElement.classList.add('active');
    } else {
        isClickEventActive = false;
        nextlv2.classList.remove('fa-minus');
        nextlv2.parentElement.classList.remove('active');
    }
});