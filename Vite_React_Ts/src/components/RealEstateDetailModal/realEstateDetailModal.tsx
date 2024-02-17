import { Carousel } from "@material-tailwind/react";
import { fromAddress, setKey, geocode, RequestType } from "react-geocode";
import { useJsApiLoader, GoogleMap, Marker } from "@react-google-maps/api";
import { useEffect, useRef, useState } from "react";

interface RealEstateDetailModalProps {
  realEstateId: number;
  closeModal: () => void;
  address: string;
}

const mapOptions = {
  disableDefaultUI: true, // Disable default UI controls
  mapTypeControl: false, // Disable map type control
  draggable: false, // Disable dragging
};

const RealEstateDetailModal = ({
  closeModal,
  realEstateId,
  address,
}: RealEstateDetailModalProps) => {
  const [center, setCenter] = useState({ lat: 40.7128, lng: -74.006 });
  console.log(address);
  // Use Geocode to convert address to coordinates

  const { isLoaded } = useJsApiLoader({
    googleMapsApiKey: process.env.REACT_APP_GOOGLE_MAPS_API_KEY as string,
  });

  setKey(process.env.REACT_APP_GOOGLE_MAPS_API_KEY as string);

  const addresss = "1600 Amphitheatre Parkway, Mountain View, CA";
  geocode(RequestType.ADDRESS, addresss)
    .then((response) => {
      console.log(response);
    })
    .catch((error) => {
      console.error(error);
    });

  useEffect(() => {
    geocode(RequestType.ADDRESS, address)
      .then(({ results }) => {
        const { lat, lng } = results[0].geometry.location;
        console.log({ lat, lng });
        setCenter({ lat, lng });
      })
      .catch(console.error);
  }, [address]);

  // useEffect(() => {
  //   if (mapRef.current && center) {
  //     const map = mapRef.current
  //     setCenter(map)
  //   }
  // }, [center]);

  const [tabStatus, setTabStatus] = useState(1);

  // Change the tab index
  const toggleTab = (index: number) => {
    setTabStatus(index);
  };

  // Tab button status
  const getActiveTab = (index: number) => {
    return `${
      index === tabStatus
        ? "text-mainBlue border-mainBlue font-bold"
        : "border-transparent hover:border-gray-900"
    } text-xl  border-b-2 rounded-t-lg`;
  };

  // Changing the tab description
  const getActiveTabDetail = (index: number) => {
    return `${index === tabStatus ? "" : "hidden"} mt-2 space-y-4 `;
  };

  return isLoaded ? (
    <div className="relative w-full max-w-7xl max-h-full ">
      <div className="relative bg-white rounded-lg shadow md:px-10 md:pb-5 sm:px-0 sm:pb-0 ">
        <div className=" items-center justify-start md:py-5 md:px-0 sm:p-5 sm:fixed md:static z-10 top-0">
          <button
            type="button"
            className=" bg-transparent md:bg-transparent sm:bg-white sm:bg-opacity-60 rounded-3xl text-sm w-10 h-10 ms-auto inline-flex justify-center items-center "
            data-modal-hide="default-modal"
            onClick={closeModal}
          >
            <svg
              className="w-6 h-6  sm:text-black sm:hover:text-mainBlue "
              aria-hidden="true"
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 14 10"
            >
              <path
                stroke="currentColor"
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="2"
                d="M13 5H1m0 0 4 4M1 5l4-4"
              />
            </svg>
            <span className="sr-only">Close modal</span>
          </button>
        </div>

        <div className="grid md:grid-cols-2 gap-8">
          <div className="top-5">
            <Carousel className="h-full">
              <img
                src="https://flowbite.s3.amazonaws.com/docs/gallery/square/image-1.jpg"
                alt=""
                className="h-80 w-full object-cover rounded-lg"
              />
              <img
                src="https://flowbite.s3.amazonaws.com/docs/gallery/square/image-2.jpg"
                alt=""
                className="h-80 w-full object-cover rounded-lg"
              />
              <img
                src="https://flowbite.s3.amazonaws.com/docs/gallery/square/image-3.jpg"
                alt=""
                className="h-80 w-full object-cover rounded-lg"
              />
              <img
                src="https://flowbite.s3.amazonaws.com/docs/gallery/square/image-4.jpg"
                alt=""
                className="h-80 w-full object-cover rounded-lg"
              />
              <img
                src="https://flowbite.s3.amazonaws.com/docs/gallery/square/image-5.jpg"
                alt=""
                className="h-80 w-full object-cover rounded-lg"
              />
            </Carousel>
          </div>
          <div className=" h-full rounded-lg">
            <GoogleMap
              mapContainerStyle={{
                width: "100%",
                height: "320px",
                borderRadius: "0.5rem",
              }}
              center={center}
              zoom={15}
              mapTypeId={"terrain"}
              options={mapOptions}
            >
              <Marker position={center} />
            </GoogleMap>
          </div>
        </div>
        <div className=" md:mb-0  sm:px-4 md:px-0">
          <hr className="mt-8 mb-6 border-gray-200 sm:mx-auto " />
          <div className="">
            <div className="text-4xl font-bold">Giant Mansion</div>
            <div>
              <ul className="mt-2 flex flex-row gap-4">
                <li>
                  <button
                    className={getActiveTab(1)}
                    onClick={() => toggleTab(1)}
                  >
                    Detail
                  </button>
                </li>
                <li>
                  <button
                    className={getActiveTab(2)}
                    onClick={() => toggleTab(2)}
                  >
                    Auction
                  </button>
                </li>
              </ul>
            </div>
          </div>
          <div className={getActiveTabDetail(1)}>
            <p className="text-base leading-relaxed text-gray-900">
              With less than a month to go before the European Union enacts new
              consumer privacy laws for its citizens, companies around the world
              are updating their terms of service agreements to comply.
            </p>
            <p className="text-base leading-relaxed text-gray-900">
              With less than a month to go before the European Union enacts new
              consumer privacy laws for its citizens, companies around the world
              are updating their terms of service agreements to comply.
            </p>
          </div>
          <div className={getActiveTabDetail(2)}>
            <p className="text-base leading-relaxed text-gray-900">
              With less than a month to go before the European Union enacts new
              consumer privacy laws for its citizens, companies around the world
              are updating their terms of service agreements to comply.
            </p>
            <p className="text-base leading-relaxed text-gray-900">
              With less than a month to go before the European Union enacts new
              consumer privacy laws for its citizens, companies around the world
              are updating their terms of service agreements to comply.
            </p>
            <p className="text-base leading-relaxed text-gray-900">
              With less than a month to go before the European Union enacts new
              consumer privacy laws for its citizens, companies around the world
              are updating their terms of service agreements to comply.
            </p>
          </div>
        </div>
        <footer>
          <div className="w-full max-w-screen-xl">
            <hr className="my-6 border-gray-200 sm:mx-auto lg:my-8 " />
            <span className="block text-sm text-gray-900 sm:text-center xs:text-center sm:pb-8 md:pb-0">
              © 2023 REAS™ . All Rights Reserved.
            </span>
          </div>
        </footer>
      </div>
    </div>
  ) : (
    <></>
  );
};

export default RealEstateDetailModal;