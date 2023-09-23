    const slides = document.querySelectorAll('.slide');
    let currentSlide = 0;

    function adjustFontSize(slide) {
        const content = slide.querySelector('.slide-content');
        if (!content) return;  // Om det inte finns något innehåll, hoppa över justering

        let fontSize = 2; // startvärde i rem
        slide.style.fontSize = fontSize + 'rem';
        while (content.scrollHeight > content.offsetHeight && fontSize > 0.5) {
            fontSize -= 0.1;
            slide.style.fontSize = fontSize + 'rem';
        }
    }

    function showSlide(index) {
        if (index >= 0 && index < slides.length) {
            slides[currentSlide].style.display = "none"; // Dölj nuvarande slide
            currentSlide = index;
            slides[currentSlide].style.display = "block"; // Visa nästa slide
            adjustFontSize(slides[currentSlide]);
        }
    }

    document.body.addEventListener('keydown', function (e) {
        if (e.key === 'ArrowRight' || e.key === ' ') {
            showSlide(currentSlide + 1);
        } else if (e.key === 'ArrowLeft') {
            showSlide(currentSlide - 1);
        }
    });

    adjustFontSize(slides[0]);

    showSlide(0);  // Sätt den första sliden som aktiv när sidan laddas