export default function Loading({ text = 'Yükleniyor...' }) {
    return (
        <div className="loading-container">
            <div className="spinner" />
            <span className="loading-text">{text}</span>
        </div>
    );
}
