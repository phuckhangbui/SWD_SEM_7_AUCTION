import { useEffect, useState } from "react";
import news from "../../interface/news";
import NewsCard from "./newsCard";
import NewsDetailModal from "../NewsDetailModal/newsDetailModal";

interface NewsListProp {
  newsList: news[];
}

const NewsList = ({ newsList }: NewsListProp) => {
  const [news, setNews] = useState(newsList);
  const [showModal, setShowModal] = useState(false);
  const [newsId, setNewsId] = useState<number>(-1);

  const toggleModal = (newsId: number) => {
    setShowModal((prevShowModal) => !prevShowModal);
    setNewsId(newsId);
  };

  useEffect(() => {
    // Disable scroll on body when modal is open
    if (showModal) {
      document.body.style.overflow = "hidden";
    } else {
      document.body.style.overflow = "auto";
    }

    // Cleanup function
    return () => {
      document.body.style.overflow = "auto";
    };
  }, [showModal]);

  const closeModal = () => {
    setShowModal(!showModal);
  };

  const handleOverlayClick = (e: React.MouseEvent<HTMLDivElement>) => {
    if (e.target === e.currentTarget) {
      // If the click occurs on the overlay (not on the modal content), close the modal
      closeModal();
    }
  };

  return (
    <div>
      <div>
        <div className="mt-4 grid lg:grid-cols-2 md:grid-cols-2 md:gap-3 sm:grid-cols-1">
          {news.map((news) => (
            <div
              key={news.id}
              onClick={() => toggleModal(news.id)}
              // type="button"
            >
              <NewsCard news={news} />
            </div>
          ))}
        </div>
      </div>
      {showModal && (
        <div
          id="default-modal"
          tabIndex={-1}
          aria-hidden="true"
          className=" fixed top-0 left-0 right-0 inset-0 overflow-x-hidden overflow-y-auto z-50 flex items-center justify-center bg-black bg-opacity-50 w-full max-h-full md:inset-0 "
          onMouseDown={handleOverlayClick}
        >
          <NewsDetailModal
            closeModal={closeModal}
            newsId={newsId}
          />
        </div>
        // <div id="extralarge-modal" tabindex="-1" class="fixed top-0 left-0 right-0 z-50 hidden w-full p-4 overflow-x-hidden overflow-y-auto md:inset-0 h-[calc(100%-1rem)] max-h-full">
      )}
    </div>
  );
};

export default NewsList;