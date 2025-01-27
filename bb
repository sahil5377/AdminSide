"use client";

import ShareModal from '@/components/ShareModal/ShareModal';
import { GroupCategoryInterface, Posts } from '@/services/modules/MarketingCenter/interfaces';
import { IMAGE_FORMATS, SVGAdapter } from '@/utils';
import { CardMedia } from '@mui/material';
import { memo, MouseEvent, useEffect, useState } from 'react';
import Style from './StaticCategory.module.scss';

interface ProductProps {
  title: string,
  selectedItem: Posts | GroupCategoryInterface,
  imageUrl?: string
}

const DEFAULT_IMAGE_URL = "https://www.yourmarketingagent.com/img/site_specific/uploads/alliance/files/crop_Flyer_Thumb_image.png";
const CDN_URL = process.env.NEXT_PUBLIC_CDN_URL;

export default memo(function StaticCategory({
  title,
  selectedItem,
  imageUrl
}: ProductProps) {
  const [openShare, setOpenShare] = useState(false);
  const [shareUrl, setShareUrl] = useState<string | undefined>();
  const currentImage = imageUrl ? imageUrl : DEFAULT_IMAGE_URL;
  const imageStyle = {
    backgroundImage: "url(" + currentImage + ")",
    backgroundSize: 'cover',
  }

  const [isSvg, setIsSvg] = useState<boolean>(false);
  const [image, setImage] = useState<string>('');

  const downloadExampleFile = async (linkUrl: string, fileName: string) => {
    const fileExtension = linkUrl?.split('.')?.pop();
    console.log(linkUrl, CDN_URL);

    if (!linkUrl.includes(CDN_URL) && IMAGE_FORMATS.includes(fileExtension)) {
      window.open(linkUrl, '_blank');
      return;
    }

    const response = await fetch(linkUrl);
    const blob = await response.blob();
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', decodeURIComponent(fileName));
    document.body.appendChild(link);
    link.click();
    link.remove();
    window.URL.revokeObjectURL(url);
  }

  const handleShareClose = () => {
    setOpenShare(false);
    setShareUrl(undefined);
  }

  useEffect(() => {
    const fetchSVG = async () => {
      try {
        if (imageUrl?.endsWith('.svg')) {
          setIsSvg(true);
          const modifiedSVG = await SVGAdapter(imageUrl)
          setImage(modifiedSVG);
        } else {
          setIsSvg(false);
          setImage(imageUrl);
        }
      } catch (error) {
        console.error('Error loading SVG:', error);
      }
    };

    fetchSVG();
  }, [imageUrl]);

  const handleTeaserClick = (event: MouseEvent<HTMLDivElement>) => {
    event.preventDefault();
    const linkElement = (event.target as HTMLElement).closest('a');
    if (linkElement) {
      const linkUrl = linkElement.href;
      if (linkUrl === "javascript:;") return;
      if (linkElement.target === "_blank") {
        window.open(linkUrl, '_blank');
        return;
      }
      const fileName = linkUrl.split('/').pop() || "file.pdf";
      downloadExampleFile(linkUrl, fileName);
    }
  };

  useEffect(() => {
    window["share_tofb"] = (url: string) => {
      setShareUrl(url);
      setOpenShare(true);
    }
  }, []);

  return (
    <div
      className={Style.designContainer}
    >
      <div className={Style.description}>
        {
          isSvg ? (
            <CardMedia
              className={`${Style['image-card']}`}
              component="div"
              dangerouslySetInnerHTML={{ __html: image }}
            />
          ) : (
            !imageUrl ? (
              <div className={Style.image} style={imageStyle}></div>
            ) : (
              <CardMedia
                component="img"
                image={currentImage}
                alt={title}
                className={Style['image-card']}
              />
            )
          )
        }
      </div>
      <div className={Style.titleContainer}><p className={`h2-font ${Style.title}`}>{title}</p></div>
      <div className='generatedButton' onClick={handleTeaserClick}>
        <div
          dangerouslySetInnerHTML={{ __html: selectedItem.teaser }}
          className={Style.downloadButton}
        />
      </div>
      <ShareModal imageUrl={shareUrl} open={openShare} title='Share your Item' onClose={handleShareClose}></ShareModal>
    </div>
  )
})
