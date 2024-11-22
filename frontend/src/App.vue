<template>
  <div>
    <h2>Upload Image</h2>
    <input type="file" @change="onFileChange" accept="image/*" />
    <button @click="uploadImage" :disabled="!base64Image">Upload</button>

    <p v-if="imageUrl">Image uploaded successfully! <a :href="imageUrl" target="_blank">View Image</a></p>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  data() {
    return {
      selectedFile: null,
      base64Image: '',  // Base64 string of the selected image
      imageUrl: '', // URL of the uploaded image (optional, for preview)
    };
  },
  methods: {
    // Handle file selection
    onFileChange(event) {
      const file = event.target.files[0];
      if (file) {
        this.selectedFile = file;
        this.convertToBase64(file);
      }
    },

    // Convert the image file to base64 string
    convertToBase64(file) {
      const reader = new FileReader();
      reader.onloadend = () => {
        this.base64Image = reader.result.split(',')[1]; // Remove the "data:image/png;base64," prefix
      };
      reader.readAsDataURL(file);
    },

    // Call the backend API to upload the image
    async uploadImage() {
      if (this.base64Image) {
        try {
          const response = await axios.post('http://localhost:5128/api/ImageUpload/upload', {
            Base64Image: this.base64Image,
          });

          // Handle the successful upload response
          this.imageUrl = response.data.FilePath;
        } catch (error) {
          console.error('Error uploading image:', error);
        }
      }
    },
  },
};
</script>

<style scoped>
/* Add some basic styling */
</style>
