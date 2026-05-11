// ==================== GLOBAL PLAYER ====================
const PlayerApp = (function () {
    let audio = null;
    let playlist = []; // Текущий плейлист страницы
    let currentSongDetails = null; // Детали текущей воспроизводимой песни
    let savedVolume = 1;

    const LOCAL_STORAGE_KEY = 'vladifyPlayerState';

    function init() {
        audio = document.getElementById('audioPlayer');
        if (!audio) return;

        // Загрузка сохраненного состояния
        loadPlayerState();

        // Устанавливаем громкость
        audio.volume = savedVolume;
        updateVolumeUI(savedVolume);

        // Обновляем плеер, если есть сохраненная песня
        if (currentSongDetails) {
            updatePlayerBar(currentSongDetails);
            showPlayer();
            // На этом этапе аудио.src может быть не установлен или не готов
            // Мы не вызываем audio.play() здесь, чтобы не было автовоспроизведения при загрузке страницы
            // Пользователь должен нажать кнопку "play"
            audio.src = currentSongDetails.audioUrl;
        }

        // Event Listeners for Audio
        audio.addEventListener('timeupdate', onTimeUpdate);
        audio.addEventListener('loadedmetadata', onLoadedMetadata);
        audio.addEventListener('ended', playNext);
        audio.addEventListener('play', () => updateAllUI(true));
        audio.addEventListener('pause', () => updateAllUI(false));
        audio.addEventListener('volumechange', () => savePlayerState()); // Сохраняем громкость

        // Progress bar drag
        const progressBar = document.getElementById('progressBarContainer');
        if (progressBar) {
            let dragging = false;
            progressBar.addEventListener('mousedown', e => { dragging = true; seekTo(e); });
            document.addEventListener('mousemove', e => { if (dragging) seekTo(e); });
            document.addEventListener('mouseup', () => { dragging = false; });
        }

        // Volume bar drag
        const volumeBar = document.getElementById('volumeBarContainer');
        if (volumeBar) {
            let dragging = false;
            volumeBar.addEventListener('mousedown', e => { dragging = true; setVolume(e); });
            document.addEventListener('mousemove', e => { if (dragging) setVolume(e); });
            document.addEventListener('mouseup', () => { dragging = false; });
        }
    }

    // ==================== STATE PERSISTENCE ====================
    function savePlayerState() {
        const state = {
            currentSongDetails: currentSongDetails,
            currentTime: audio ? audio.currentTime : 0,
            volume: audio ? audio.volume : 1,
            isPlaying: audio ? !audio.paused : false
        };
        localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(state));
    }

    function loadPlayerState() {
        const savedState = localStorage.getItem(LOCAL_STORAGE_KEY);
        if (savedState) {
            const state = JSON.parse(savedState);
            currentSongDetails = state.currentSongDetails;
            savedVolume = state.volume;
            // currentTime и isPlaying будут использоваться после loadedmetadata,
            // но мы не будем авто-воспроизводить на каждой загрузке страницы.
            // Их можно использовать для восстановления положения при ручном воспроизведении.
        }
    }

    // ==================== PLAYLIST ====================
    // Загружает плейлист из HTML-строк страницы
    function loadPlaylistFromRows(selector) {
        const rows = document.querySelectorAll(selector || '.song-row[data-song-audio]');
        playlist = Array.from(rows).map((row, idx) => ({
            index: idx, // Индекс в этом конкретном плейлисте
            id: row.dataset.songId,
            title: row.dataset.songTitle,
            author: row.dataset.songAuthor,
            album: row.dataset.songAlbum || '',
            audioUrl: row.dataset.songAudio,
            imageUrl: row.dataset.songImage || '',
            duration: row.dataset.songDuration || ''
        }));
        // После загрузки плейлиста, переподсвечиваем строки, если текущая песня есть в новом плейлисте
        if (currentSongDetails) {
            const idxInNewPlaylist = playlist.findIndex(s => s.id === currentSongDetails.id);
            highlightRows(idxInNewPlaylist, audio && !audio.paused);
        }
    }

    // Устанавливает плейлист программно (например, для поиска)
    function setPlaylist(songsArray) {
        playlist = songsArray.map((s, i) => ({
            index: i,
            id: s.id,
            title: s.title,
            author: s.author,
            album: s.album || '',
            audioUrl: s.audioUrl,
            imageUrl: s.imageUrl || '',
            duration: s.duration || ''
        }));
        // При программной установке плейлиста, если текущая песня есть в нем, обновляем highlight
        if (currentSongDetails) {
            const idxInNewPlaylist = playlist.findIndex(s => s.id === currentSongDetails.id);
            highlightRows(idxInNewPlaylist, audio && !audio.paused);
        }
    }

    // ==================== PLAY ====================
    function playSongByIndex(idx) {
        if (idx < 0 || idx >= playlist.length) return;

        const song = playlist[idx];
        currentSongDetails = song;

        audio.src = song.audioUrl;
        audio.currentTime = 0;  // ← гарантируем начало с нуля
        audio.play();

        updatePlayerBar(song);
        showPlayer();
        highlightRows(idx, true);
        savePlayerState();
    }

    function playById(songId) {
        const idx = playlist.findIndex(s => s.id === songId);
        if (idx >= 0) playSongByIndex(idx);
        else {
            // Если песня не найдена в текущем плейлисте, возможно, она была загружена из localStorage
            // и пользователь кликнул "play" на глобальном плеере.
            // В этом случае, если currentSongDetails соответствует songId, просто воспроизводим его.
            if (currentSongDetails && currentSongDetails.id === songId) {
                audio.play();
            }
        }
    }

    // Клик по строке — play/pause toggle
    function onRowClick(idx) {
        // Если кликнули по той же песне которая играет — ставим на паузу
        if (currentSongDetails && idx === currentSongDetails.index && !audio.paused) {
            audio.pause();
            savePlayerState();
            return;
        }
        // Если кликнули по той же песне на паузе — играем СНАЧАЛА
        if (currentSongDetails && idx === currentSongDetails.index && audio.paused) {
            audio.currentTime = 0;
            audio.play();
            savePlayerState();
            return;
        }
        // Новая песня — playSongByIndex уже всегда начинает сначала (так как меняется audio.src)
        playSongByIndex(idx);
    }

    // ==================== CONTROLS ====================
    function togglePlayPause() {
        if (audio.paused) {
            // Если src не установлен (например, при первой загрузке или после ошибки)
            if (!audio.src || audio.src === window.location.href) {
                // Пытаемся воспроизвести последнюю сохраненную песню
                if (currentSongDetails) {
                    audio.src = currentSongDetails.audioUrl;
                    audio.play();
                    // Восстанавливаем время, если есть
                    const savedState = JSON.parse(localStorage.getItem(LOCAL_STORAGE_KEY));
                    if (savedState && savedState.currentTime > 0) {
                        audio.currentTime = savedState.currentTime;
                    }
                    updatePlayerBar(currentSongDetails);
                    showPlayer();
                    highlightRows(currentSongDetails.index, true); // Подсвечиваем, если песня есть в текущем плейлисте
                } else if (playlist.length > 0) { // Если нет сохраненной песни, играем первую из текущего плейлиста
                    playSongByIndex(0);
                }
            } else {
                audio.play();
            }
        } else {
            audio.pause();
        }
        savePlayerState(); // Сохраняем состояние play/pause
    }


    function playNext() {
        if (playlist.length === 0) return;
        let nextIndex = (currentSongDetails ? currentSongDetails.index : -1) + 1;
        if (nextIndex >= playlist.length) nextIndex = 0; // Зацикливание
        playSongByIndex(nextIndex);
    }

    function playPrev() {
        if (playlist.length === 0) return;
        if (audio.currentTime > 3) { // Если песня играет больше 3 секунд, просто перезапускаем ее
            audio.currentTime = 0;
            return;
        }
        let prevIndex = (currentSongDetails ? currentSongDetails.index : 0) - 1;
        if (prevIndex < 0) prevIndex = playlist.length - 1; // Зацикливание
        playSongByIndex(prevIndex);
    }

    function togglePlayAll() {
        // Кнопка Play/Pause на странице, если текущий плейлист пуст, то ничего не делаем.
        if (playlist.length === 0) return;

        if (audio.paused) {
            // Если ничего не играет, начинаем с первой песни текущего плейлиста
            if (!currentSongDetails) {
                playSongByIndex(0);
            } else { // Если что-то было на паузе, продолжаем
                audio.play();
            }
        } else { // Если играет, ставим на паузу
            audio.pause();
        }
        savePlayerState();
    }

    // ==================== PROGRESS ====================
    function onTimeUpdate() {
        if (!audio.duration) return;
        const pct = (audio.currentTime / audio.duration) * 100;
        const fill = document.getElementById('progressFill');
        const thumb = document.getElementById('progressThumb');
        const timeEl = document.getElementById('playerCurrentTime');
        if (fill) fill.style.width = pct + '%';
        if (thumb) thumb.style.left = pct + '%';
        if (timeEl) timeEl.textContent = formatTime(audio.currentTime);

        // Сохраняем текущее время (не слишком часто, чтобы не нагружать localStorage)
        if (Math.floor(audio.currentTime) % 5 === 0) { // Каждые 5 секунд
            savePlayerState();
        }
    }

    function onLoadedMetadata() {
        const dur = document.getElementById('playerDuration');
        if (dur) dur.textContent = formatTime(audio.duration);

       /* // После загрузки метаданных, если была сохраненная позиция, восстанавливаем ее
        const savedState = JSON.parse(localStorage.getItem(LOCAL_STORAGE_KEY));
        if (savedState && savedState.currentSongDetails && savedState.currentSongDetails.id === currentSongDetails.id && savedState.currentTime > 0) {
            audio.currentTime = savedState.currentTime;
        }
        // Если была playing, запускаем (но это может быть проблематично на перезагрузке страницы, лучше ручной запуск)
        // if (savedState && savedState.isPlaying && audio.paused) {
        //     audio.play();
        // }*/
    }

    function seekTo(e) {
        const bar = document.getElementById('progressBarContainer');
        if (!bar || !audio.duration) return;
        const rect = bar.getBoundingClientRect();
        const pct = Math.max(0, Math.min(1, (e.clientX - rect.left) / rect.width));
        audio.currentTime = pct * audio.duration;
        savePlayerState();
    }

    // ==================== VOLUME ====================
    function setVolume(e) {
        const bar = document.getElementById('volumeBarContainer');
        if (!bar) return;
        const rect = bar.getBoundingClientRect();
        const pct = Math.max(0, Math.min(1, (e.clientX - rect.left) / rect.width));
        audio.volume = pct;
        savedVolume = pct; // Обновляем savedVolume для localStorage
        updateVolumeUI(pct);
        savePlayerState();
    }

    function toggleMute() {
        if (audio.volume > 0) {
            savedVolume = audio.volume; // Сохраняем текущую громкость перед мутом
            audio.volume = 0;
            updateVolumeUI(0);
        } else {
            audio.volume = savedVolume; // Восстанавливаем сохраненную громкость
            updateVolumeUI(savedVolume);
        }
        savePlayerState();
    }

    function updateVolumeUI(vol) {
        const fill = document.getElementById('volumeFill');
        const thumb = document.getElementById('volumeThumb');
        const iconUp = document.getElementById('iconVolumeUp');
        const iconMute = document.getElementById('iconVolumeMute');
        if (fill) fill.style.width = (vol * 100) + '%';
        if (thumb) thumb.style.left = (vol * 100) + '%';
        if (iconUp) iconUp.classList.toggle('d-none', vol === 0);
        if (iconMute) iconMute.classList.toggle('d-none', vol > 0);
    }

    // ==================== UI UPDATES ====================
    function updatePlayerBar(song) {
        const cover = document.getElementById('playerCover');
        const title = document.getElementById('playerTitle');
        const author = document.getElementById('playerAuthor');
        if (cover) cover.src = song.imageUrl || '';
        if (title) title.textContent = song.title;
        if (author) author.textContent = song.author;
    }

    function showPlayer() {
        const bar = document.getElementById('playerBar');
        if (bar) bar.classList.add('visible');
    }

    function updateAllUI(isPlaying) {
        // Player bar play/pause icons
        const iconPlay = document.getElementById('iconPlay');
        const iconPause = document.getElementById('iconPause');
        if (iconPlay) iconPlay.classList.toggle('d-none', isPlaying);
        if (iconPause) iconPause.classList.toggle('d-none', !isPlaying);

        // Main play button (if exists on page)
        const mainBtn = document.getElementById('mainPlayBtn');
        if (mainBtn) {
            mainBtn.classList.toggle('playing', isPlaying);
            mainBtn.classList.toggle('paused', !isPlaying);
        }

        // Highlight rows (обновляем только если текущая песня в плейлисте страницы)
        if (currentSongDetails) {
            const idxInCurrentPlaylist = playlist.findIndex(s => s.id === currentSongDetails.id);
            highlightRows(idxInCurrentPlaylist, isPlaying);
        } else {
            highlightRows(-1, isPlaying); // Снимаем все подсветки, если нет текущей песни
        }
    }

    function highlightRows(idx, isPlaying) {
        const rows = document.querySelectorAll('.song-row[data-song-index]');
        rows.forEach(row => {
            const i = parseInt(row.dataset.songIndex);
            const isActive = i === idx;
            row.classList.toggle('active-song', isActive);
            row.classList.toggle('playing-song', isActive && isPlaying);
            row.classList.toggle('paused-song', isActive && !isPlaying);
        });
    }

    // ==================== HELPERS ====================
    function formatTime(sec) {
        if (!sec || isNaN(sec) || sec < 0) return '0:00';
        const m = Math.floor(sec / 60);
        const s = Math.floor(sec % 60);
        return m + ':' + (s < 10 ? '0' : '') + s;
    }

    function getCurrentSongId() { return currentSongDetails ? currentSongDetails.id : null; }
    function isPlaying() { return audio && !audio.paused; }
    function getAudio() { return audio; }

    return {
        init,
        loadPlaylistFromRows,
        setPlaylist,
        playSongByIndex,
        playById,
        onRowClick,
        togglePlayPause,
        playNext,
        playPrev,
        togglePlayAll,
        toggleMute,
        getCurrentSongId,
        isPlaying,
        getAudio,
        updateAllUI // Чтобы внешние скрипты могли вызвать обновление UI при изменении состояния
    };
})();
// Делаем доступным глобально
window.PlayerApp = PlayerApp;

document.addEventListener('DOMContentLoaded', () => PlayerApp.init());