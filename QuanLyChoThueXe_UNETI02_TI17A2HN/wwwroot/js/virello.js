/* ================================================================
   File: wwwroot/js/virello.js
   Họ và tên: Vũ Tiến Đạt - MSSV: 23103100119 - SV3
   Nội dung: Hiệu ứng theo mẫu VIRELLO: menu mobile, reveal khi cuộn,
             parallax ảnh hero, lọc xe, mở/đóng modal.
   ================================================================ */
(function () {
    'use strict';

    function qs(s, c) { return (c || document).querySelector(s); }
    function qsa(s, c) { return Array.prototype.slice.call((c || document).querySelectorAll(s)); }

    /* ---- Biểu đồ (Chart.js) theo nền tối ---- */
    if (window.Chart && window.Chart.defaults) {
        Chart.defaults.color = '#94a3b8';
        Chart.defaults.borderColor = 'rgba(255, 255, 255, 0.08)';
        Chart.defaults.font.family = "'Inter', sans-serif";
    }

    /* ---- Menu mobile ---- */
    var burger = qs('#vrMobileBtn');
    var mobile = qs('#vrMobileMenu');
    if (burger && mobile) {
        burger.addEventListener('click', function () {
            var open = mobile.classList.toggle('open');
            burger.setAttribute('aria-expanded', open ? 'true' : 'false');
        });
    }

    /* ---- Reveal khi cuộn (fade-up) ---- */
    function reveal() {
        var h = window.innerHeight;
        qsa('.vr-reveal').forEach(function (el) {
            if (el.getBoundingClientRect().top < h - 80) el.classList.add('active');
        });
    }
    window.addEventListener('scroll', reveal, { passive: true });
    window.addEventListener('resize', reveal);
    window.addEventListener('load', reveal);
    reveal();

    /* ---- Parallax ảnh nền hero ---- */
    var heroImg = qs('#vrHeroBg img');
    if (heroImg) {
        var ticking = false;
        window.addEventListener('scroll', function () {
            if (ticking) return;
            ticking = true;
            window.requestAnimationFrame(function () {
                heroImg.style.transform = 'scale(1.05) translateY(' + (window.scrollY * 0.3) + 'px)';
                ticking = false;
            });
        }, { passive: true });
    }

    /* ---- Lọc xe theo loại ---- */
    var filterBtns = qsa('.vr-filter-btn');
    filterBtns.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var loai = btn.getAttribute('data-filter');
            filterBtns.forEach(function (b) { b.classList.remove('active'); });
            btn.classList.add('active');
            qsa('.vr-card[data-loai]').forEach(function (card) {
                var hien = loai === 'all' || card.getAttribute('data-loai') === loai;
                card.style.display = hien ? '' : 'none';
            });
        });
    });

    /* ---- Modal ---- */
    function moModal(id) {
        var m = document.getElementById(id);
        if (m) { m.classList.add('open'); m.setAttribute('aria-hidden', 'false'); }
    }
    function dongModal(m) {
        if (m) { m.classList.remove('open'); m.setAttribute('aria-hidden', 'true'); }
    }
    qsa('[data-vr-open]').forEach(function (el) {
        el.addEventListener('click', function (e) {
            e.preventDefault();
            moModal(el.getAttribute('data-vr-open'));
        });
    });
    qsa('.vr-modal').forEach(function (m) {
        m.addEventListener('click', function (e) {
            if (e.target === m || e.target.closest('[data-vr-close]')) dongModal(m);
        });
    });
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') qsa('.vr-modal.open').forEach(dongModal);
    });
})();
